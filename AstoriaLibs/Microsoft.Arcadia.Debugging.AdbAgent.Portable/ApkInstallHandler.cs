using System;
using System.Text;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ApkInstallHandler : IAdbPacketHandler
	{
		private IAdbChannelClientManager channelManager;

		private IFactory factory;

		private AppxPackageType appxPackageType;

		private AdbPacketSendWork adbServerSender;

		private ChannelJobDispatcher dispatcher;

		public ApkInstallHandler(IAdbChannelClientManager channelManager, IFactory factory, AppxPackageType appxPackageType, AdbPacketSendWork adbServerSender, ChannelJobDispatcher dispatcher)
		{
			if (channelManager == null)
			{
				throw new ArgumentNullException("channelManager");
			}
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (adbServerSender == null)
			{
				throw new ArgumentNullException("adbServerSender");
			}
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}
			this.factory = factory;
			this.channelManager = channelManager;
			this.appxPackageType = appxPackageType;
			this.adbServerSender = adbServerSender;
			this.dispatcher = dispatcher;
		}

		bool IAdbPacketHandler.HandlePacket(AdbPacket receivedPacket)
		{
			if (receivedPacket == null)
			{
				throw new ArgumentNullException("receivedPacket");
			}
			ShellPmInstallParam shellPmInstallParam = null;
			if (receivedPacket.Command == 1313165391)
			{
				string command = AdbPacket.ParseStringFromData(receivedPacket.Data);
				shellPmInstallParam = ShellPmInstallParam.ParseFromOpen(command);
			}
			else if (factory.AgentConfiguration.EnableInteractiveShell && receivedPacket.Command == 1163154007 && InteractiveShellChannels.ChannelExists(receivedPacket.Arg0, receivedPacket.Arg1))
			{
				string @string = Encoding.UTF8.GetString(receivedPacket.Data, 0, receivedPacket.Data.Length);
				shellPmInstallParam = ShellPmInstallParam.ParseFromInteractiveShell(@string);
			}
			if (shellPmInstallParam != null)
			{
				StartNewInstallJob(receivedPacket, shellPmInstallParam);
				return true;
			}
			return false;
		}

		private void StartNewInstallJob(AdbPacket receivedPacket, ShellPmInstallParam param)
		{
			ApkInstallJob job = new ApkInstallJob(param, factory, appxPackageType);
			dispatcher.ExecuteJob(job, adbServerSender, channelManager, receivedPacket.Arg0, receivedPacket.Arg1);
		}
	}
}
