using System;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
	public sealed class SocketAcceptWork : ISocketAcceptWork, IWork, IDisposable
	{
		private enum StateValue
		{
			NotStarted,
			WaitingForConnection,
			ConnectionReceived,
			Error
		}

		private AutoResetEvent signalHandle = new AutoResetEvent(initialState: true);

		private StateValue state;

		private string hostName;

		private int port;

		private StreamSocketListener listener;

		private Task listeningTask;

		private StreamSocket incomingSocket;

		WaitHandle IWork.SignalHandle => signalHandle;

		public event EventHandler<SocketAcceptedEventArgs> SocketAccepted;

		public event EventHandler ListenStarted;

		public SocketAcceptWork(string hostName, int port)
		{
			if (port <= 0)
			{
				throw new ArgumentException("Invalid port number", "port");
			}
			this.hostName = hostName;
			this.port = port;
			state = StateValue.NotStarted;
		}

		void IWork.DoWork()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			switch (state)
			{
				case StateValue.NotStarted:
					{
						state = StateValue.WaitingForConnection;
						listener = new StreamSocketListener();
						StreamSocketListener @object = listener;
						WindowsRuntimeMarshal.AddEventHandler((Func<TypedEventHandler<StreamSocketListener,
							StreamSocketListenerConnectionReceivedEventArgs>,
							EventRegistrationToken>)@object.add_ConnectionReceived,
							(Action<EventRegistrationToken>)@object.remove_ConnectionReceived,
                            OnSocketConnectionReceived);

						HostName val = ((hostName != null) ? new HostName(hostName) : ((HostName)null));
						listeningTask = WindowsRuntimeSystemExtensions.AsTask(listener.BindEndpointAsync(val, port.ToString(CultureInfo.InvariantCulture)));
						listeningTask.ContinueWith(OnListeningTaskFinished);
						if (this.ListenStarted != null)
						{
							this.ListenStarted(this, EventArgs.Empty);
						}
						break;
					}
				case StateValue.ConnectionReceived:
					if (this.SocketAccepted != null)
					{
						ISocket socketAccepted = new SocketImpl(incomingSocket);
						this.SocketAccepted(this, new SocketAcceptedEventArgs(socketAccepted));
						incomingSocket = null;
					}
					listeningTask.Wait(1000);
					listeningTask = null;
					listener.Dispose();
					listener = null;
					break;
				case StateValue.Error:
					throw new AdbEngineSocketAcceptException();
				case StateValue.WaitingForConnection:
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
				if (listener != null)
				{
					listener.Dispose();
				}
				if (incomingSocket != null)
				{
					incomingSocket.Dispose();
				}
			}
		}

		private void OnSocketConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
		{
			if (args.Socket != null)
			{
				incomingSocket = args.Socket;
				state = StateValue.ConnectionReceived;
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
		}

		private void OnListeningTaskFinished(Task task)
		{
			if (task.Exception != null)
			{
				state = StateValue.Error;
				try
				{
					signalHandle.Set();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}
	}
}