using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
	public sealed class SocketConnectWork : ISocketConnectWork, IWork, IDisposable
	{
		private enum StateValue
		{
			NotStarted,
			Connecting,
			Success,
			Error
		}

		private AutoResetEvent signalHandle = new AutoResetEvent(initialState: true);

		private StateValue state;

		private string remoteAddress;

		private int remotePort;

		private StreamSocket socket;

		private uint attemptsRemaining;

		WaitHandle IWork.SignalHandle => signalHandle;

		public event EventHandler<SocketConnectedEventArgs> SocketConnected;

		public SocketConnectWork(string remoteName, int remotePort, uint attempts)
		{
			if (string.IsNullOrEmpty(remoteName))
			{
				throw new ArgumentException("host name or IP must be provided", "remoteName");
			}
			if (remotePort <= 0)
			{
				throw new ArgumentException("Invalid port number", "remotePort");
			}
			if (attempts == 0)
			{
				throw new ArgumentException("attempts must be greater than zero", "attempts");
			}
			remoteAddress = remoteName;
			this.remotePort = remotePort;
			state = StateValue.NotStarted;
			attemptsRemaining = attempts;
		}

		void IWork.DoWork()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			switch (state)
			{
				case StateValue.NotStarted:
					{
						attemptsRemaining--;
						state = StateValue.Connecting;
						socket = new StreamSocket();
						socket.Control.KeepAlive =true;
						Task task = WindowsRuntimeSystemExtensions.AsTask(socket.ConnectAsync(new HostName(remoteAddress), remotePort.ToString(CultureInfo.InvariantCulture)));
						task.ContinueWith(delegate (Task previousTask)
						{
							if (previousTask.Exception == null)
							{
								state = StateValue.Success;
							}
							else if (attemptsRemaining != 0)
							{
								socket = null;
								state = StateValue.NotStarted;
							}
							else
							{
								state = StateValue.Error;
							}
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
				case StateValue.Success:
					if (this.SocketConnected != null)
					{
						this.SocketConnected(this, new SocketConnectedEventArgs(new SocketImpl(socket)));
						socket = null;
					}
					break;
				case StateValue.Error:
					throw new AdbEngineSocketConnectException();
				case StateValue.Connecting:
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
			if (disposing)
			{
				if (signalHandle != null)
				{
					signalHandle.Dispose();
				}
				if (socket != null)
				{
					socket.Dispose();
				}
			}
		}
	}
}