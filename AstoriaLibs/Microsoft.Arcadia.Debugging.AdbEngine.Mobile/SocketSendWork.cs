using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Windows.Networking.Sockets;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
	public sealed class SocketSendWork : ISocketSendWork, IWork, IDisposable
	{
		private enum StateValue
		{
			Idle,
			Writing,
			Written,
			Flushing,
			Flushed,
			Error
		}

		private StateValue state;

		private object lockObject = new object();

		private AutoResetEvent signalHandle = new AutoResetEvent(initialState: false);

		private StreamSocket socket;

		private List<byte[]> pendingData = new List<byte[]>();

		private string identifier;

		WaitHandle IWork.SignalHandle => signalHandle;

		public SocketSendWork(ISocket socket, string identifier)
		{
			if (!(socket is SocketImpl socketImpl) || socketImpl.RealSocket == null)
			{
				throw new ArgumentException("Invalid input", "socket");
			}
			this.socket = socketImpl.RealSocket;
			this.identifier = ((identifier == null) ? string.Empty : identifier);
		}

		void IWork.DoWork()
		{
			switch (state)
			{
				case StateValue.Idle:
					{
						byte[] dataToSend = null;
						lock (lockObject)
						{
							if (pendingData.Count > 0)
							{
								dataToSend = pendingData[0];
								pendingData.RemoveAt(0);
							}
						}
						if (dataToSend == null)
						{
							break;
						}
						state = StateValue.Writing;
						Task<uint> task2 = WindowsRuntimeSystemExtensions.AsTask<uint, uint>(socket.OutputStream.WriteAsync(WindowsRuntimeBufferExtensions.AsBuffer(dataToSend)));
						task2.ContinueWith(delegate (Task<uint> previousTask)
						{
							state = ((previousTask.Exception == null && previousTask.Result == dataToSend.Length) ? StateValue.Written : StateValue.Error);
							try
							{
								signalHandle.Set();
							}
							catch (ObjectDisposedException)
							{
							}
						});
						break;
					}
				case StateValue.Written:
					{
						state = StateValue.Flushing;
						Task<bool> task = WindowsRuntimeSystemExtensions.AsTask<bool>(socket.OutputStream.FlushAsync());
						task.ContinueWith(delegate (Task<bool> previousTask)
						{
							state = ((previousTask.Exception == null && previousTask.Result) ? StateValue.Flushed : StateValue.Error);
							try
							{
								signalHandle.Set();
							}
							catch (ObjectDisposedException)
							{
							}
						});
						break;
					}
				case StateValue.Flushed:
					state = StateValue.Idle;
					signalHandle.Set();
					break;
				case StateValue.Error:
					throw new AdbEngineSocketSendReceiveException(identifier, "Socket Send Error.");
				case StateValue.Writing:
				case StateValue.Flushing:
					break;
			}
		}

		public void EnqueueForSend(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				throw new ArgumentException("data must be provided", "data");
			}
			lock (lockObject)
			{
				pendingData.Add(data);
				signalHandle.Set();
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && signalHandle != null)
			{
				signalHandle.Dispose();
			}
		}
	}
}