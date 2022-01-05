using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
	public sealed class SocketReceiveWork : ISocketReceiveWork, IWork, IDisposable
	{
		private enum StateValue
		{
			Idle,
			WaitingForData,
			DataReceiveSucceeded,
			Error
		}

		private AutoResetEvent signalHandle = new AutoResetEvent(initialState: true);

		private StateValue state;

		private StreamSocket socket;

		private byte[] buffer = new byte[262144];

		private Task<IBuffer> receivingTask;

		private string identifier;

		WaitHandle IWork.SignalHandle => signalHandle;

		public event EventHandler<SocketDataReceivedEventArgs> DataReceived;

		public SocketReceiveWork(ISocket socket, string identifier)
		{
			if (!(socket is SocketImpl socketImpl) || socketImpl.RealSocket == null)
			{
				throw new ArgumentException("Invalid input", "socket");
			}
			this.socket = socketImpl.RealSocket;
			state = StateValue.Idle;
			this.identifier = ((identifier == null) ? string.Empty : identifier);
		}

		void IWork.DoWork()
		{
			switch (state)
			{
				case StateValue.Idle:
					state = StateValue.WaitingForData;
					receivingTask = WindowsRuntimeSystemExtensions.AsTask<IBuffer, uint>(socket.InputStream.ReadAsync(WindowsRuntimeBufferExtensions.AsBuffer(buffer), (uint)buffer.Length, (InputStreamOptions)1));
					receivingTask.ContinueWith(delegate (Task<IBuffer> previousTask)
					{
						state = ((previousTask.Exception == null) ? StateValue.DataReceiveSucceeded : StateValue.Error);
						try
						{
							signalHandle.Set();
						}
						catch (ObjectDisposedException)
						{
						}
					});
					break;
				case StateValue.DataReceiveSucceeded:
					{
						IBuffer result = receivingTask.Result;
						if (result.Length == 0)
						{
							throw new AdbEngineSocketSendReceiveException(identifier, "Foreign host closed the connection.");
						}
						if (this.DataReceived != null)
						{
							byte[] data = WindowsRuntimeBufferExtensions.ToArray(result);
							this.DataReceived(this, new SocketDataReceivedEventArgs(data));
						}
						receivingTask = null;
						state = StateValue.Idle;
						signalHandle.Set();
						break;
					}
				case StateValue.Error:
					throw new AdbEngineSocketSendReceiveException(identifier, "Socket Receive Error.");
				case StateValue.WaitingForData:
					break;
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