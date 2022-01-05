using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public class AndroidDebugBridgeAgent : IDisposable
	{
		private IFactory factory;

		private IAdbTrafficMonitor adbTrafficMonitor;

		private InternetEndPoint adbDaemonEndPoint;

		private InternetEndPoint exportEndPoint;

		private WorkScheduler scheduler = new WorkScheduler();

		private ISocket adbDaemonSocket;

		private ISocket adbServerSocket;

		private ISocketConnectWork connector;

		private ISocketAcceptWork acceptor;

		private ISocketReceiveWork adbServerSocketReceiver;

		private ISocketReceiveWork adbDaemonSocketReceiver;

		private ISocketSendWork adbServerSocketSender;

		private ISocketSendWork adbDaemonSocketsender;

		private ChannelJobDispatcher jobDispatcher;

		private EventWaitHandle bootstrappedEvent = new EventWaitHandle(initialState: false, EventResetMode.ManualReset, "WConnectAgentBootstrappedEvent");

		private IList<IAdbPacketHandler> serverAdbPacketHandlers = new List<IAdbPacketHandler>();

		private IList<IAdbPacketHandler> daemonAdbPacketHandlers = new List<IAdbPacketHandler>();

		private AppxPackageType appxPackageType;

		public AndroidDebugBridgeAgent(IFactory factory, IAdbTrafficMonitor adbTrafficMonitor, InternetEndPoint adbDaemonEndPoint, InternetEndPoint exportEndPoint, AppxPackageType appxPackageType)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (adbDaemonEndPoint == null)
			{
				throw new ArgumentNullException("adbDaemonEndPoint");
			}
			if (exportEndPoint == null)
			{
				throw new ArgumentNullException("exportEndPoint");
			}
			this.factory = factory;
			this.adbTrafficMonitor = adbTrafficMonitor;
			this.adbDaemonEndPoint = adbDaemonEndPoint;
			this.exportEndPoint = exportEndPoint;
			this.appxPackageType = appxPackageType;
			jobDispatcher = new ChannelJobDispatcher();
		}

		public async Task RunAsync(CancellationToken cancellationToken)
		{
			int num = default(int);
			_ = num;
			_ = 0;
			try
			{
				while (true)
				{
					acceptor = factory.CreateSocketAcceptWork(exportEndPoint);
					acceptor.ListenStarted += OnAcceptorListenStarted;
					acceptor.SocketAccepted += OnAcceptedConnectionFromAdbServer;
					scheduler.AssignWorks(acceptor);
					try
					{
						await scheduler.RunAsync(cancellationToken);
						break;
					}
					catch (AdbEngineSocketSendReceiveException ex)
					{
						bootstrappedEvent.Reset();
						scheduler.AssignWorks();
						CloseSockets();
						DisposeObjects(connector, acceptor, adbServerSocketReceiver, adbDaemonSocketReceiver, adbServerSocketSender, adbDaemonSocketsender);
						daemonAdbPacketHandlers.Clear();
						serverAdbPacketHandlers.Clear();
						EtwLogger.Instance.SocketDataSendReceiveError(ex.SocketIdentifier, ex.Reason);
					}
				}
			}
			finally
			{
				scheduler.AssignWorks();
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private static void DisposeObjects(params IDisposable[] objectsToDispose)
		{
			for (int i = 0; i < objectsToDispose.Length; i++)
			{
				objectsToDispose[i]?.Dispose();
			}
		}

		private static void HandlePacket(AdbPacket packet, IList<IAdbPacketHandler> handlers, AdbPacketSendWork sender)
		{
			EtwLogger.Instance.AdbComandSent(packet.Command, packet.Arg0, packet.Arg1, packet.Data);
			bool flag = false;
			foreach (IAdbPacketHandler handler in handlers)
			{
				flag = handler.HandlePacket(packet);
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				sender.EnqueueForSending(packet);
			}
		}

		private void OnConnectedToAdbDaemon(object sender, SocketConnectedEventArgs e)
		{
			EtwLogger.Instance.AdbDaemonConnected();
			adbDaemonSocket = e.SocketConnected;
			adbServerSocketReceiver = factory.CreateSocketReceiveWork(adbServerSocket, "ADB server receiver");
			adbDaemonSocketReceiver = factory.CreateSocketReceiveWork(adbDaemonSocket, "ADB daemon receiver");
			adbServerSocketSender = factory.CreateSocketSendWork(adbServerSocket, "ADB server sender");
			adbDaemonSocketsender = factory.CreateSocketSendWork(adbDaemonSocket, "ADB daemon sender");
			AdbPacketReceivWork adbServerReceiver = new AdbPacketReceivWork(adbServerSocketReceiver);
			AdbPacketReceivWork adbDaemonReceiver = new AdbPacketReceivWork(adbDaemonSocketReceiver);
			AdbPacketSendWork adbServerSender = new AdbPacketSendWork(adbServerSocketSender);
			AdbPacketSendWork adbDaemonSender = new AdbPacketSendWork(adbDaemonSocketsender);
			adbServerReceiver.AdbPacketReceived += delegate (object originator, AdbPacketReceivedEventArgs param)
			{
				if (adbTrafficMonitor != null)
				{
					adbTrafficMonitor.OnAdbTraffic(param.Packet, AdbTrafficDirection.FromServer);
				}
				if (param.Packet.Command == 1314410051)
				{
					int arg = (int)param.Packet.Arg1;
					adbServerReceiver.MaxPacketBytes = arg;
					adbDaemonReceiver.MaxPacketBytes = arg;
					AdbChannelClientManager adbChannelClientManager = new AdbChannelClientManager(adbDaemonSender, arg);
					daemonAdbPacketHandlers.Add(adbChannelClientManager);
					if (factory.AgentConfiguration.EnableInteractiveShell)
					{
						serverAdbPacketHandlers.Add(new InteractiveShellTrackerHandler(directionIsFromAdbd: false));
						daemonAdbPacketHandlers.Add(new InteractiveShellTrackerHandler(directionIsFromAdbd: true));
					}
					serverAdbPacketHandlers.Add(new ApkInstallHandler(adbChannelClientManager, factory, appxPackageType, adbServerSender, jobDispatcher));
					serverAdbPacketHandlers.Add(new ApkUninstallHandler(adbChannelClientManager, factory, adbServerSender, jobDispatcher));
					serverAdbPacketHandlers.Add(new ShellActivityStartHandler(adbChannelClientManager, factory, adbServerSender, jobDispatcher));
					if (factory.AgentConfiguration.EnableInterception)
					{
						serverAdbPacketHandlers.Add(new FileSyncSnifferHandler(factory));
					}
				}
				HandlePacket(param.Packet, serverAdbPacketHandlers, adbDaemonSender);
			};
			adbDaemonReceiver.AdbPacketReceived += delegate (object s, AdbPacketReceivedEventArgs p)
			{
				if (adbTrafficMonitor != null)
				{
					adbTrafficMonitor.OnAdbTraffic(p.Packet, AdbTrafficDirection.FromDaemon);
				}
				HandlePacket(p.Packet, daemonAdbPacketHandlers, adbServerSender);
			};
			scheduler.AssignWorks(adbServerReceiver, adbDaemonReceiver, adbServerSender, adbDaemonSender);
		}

		private void OnAcceptorListenStarted(object sender, EventArgs e)
		{
			LoggerCore.Log("Socket acceptor started listening.");
			bootstrappedEvent.Set();
		}

		private void OnAcceptedConnectionFromAdbServer(object sender, SocketAcceptedEventArgs e)
		{
			EtwLogger.Instance.AdbServerAccepted();
			LoggerCore.Log("Accepted connection from ADB server.");
			adbServerSocket = e.SocketAccepted;
			connector = factory.CreateSocketConnectWork(adbDaemonEndPoint, 4u);
			connector.SocketConnected += OnConnectedToAdbDaemon;
			scheduler.AssignWorks(connector);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (bootstrappedEvent != null)
				{
					bootstrappedEvent.Reset();
					bootstrappedEvent.Dispose();
				}
				if (scheduler != null)
				{
					scheduler.AssignWorks();
					scheduler.Dispose();
					scheduler = null;
				}
				CloseSockets();
				DisposeObjects(connector, acceptor, adbServerSocketReceiver, adbDaemonSocketReceiver, adbServerSocketSender, adbDaemonSocketsender);
			}
		}

		private void CloseSockets()
		{
			if (adbDaemonSocket != null)
			{
				adbDaemonSocket.Close();
				adbDaemonSocket = null;
				EtwLogger.Instance.ForcefullyClosedDaemonSocket();
			}
			if (adbServerSocket != null)
			{
				adbServerSocket.Close();
				adbServerSocket = null;
				EtwLogger.Instance.ForcefullyClosedServerSocket();
			}
		}
	}
}
