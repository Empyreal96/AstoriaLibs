using System;
using System.Text;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ShellActivityStartHandler : IAdbPacketHandler
	{
		private IFactory factory;

		private IAdbChannelClientManager deviceChannelManager;

		private ChannelJobDispatcher dispatcher;

		private AdbPacketSendWork adbServerSender;

		public ShellActivityStartHandler(IAdbChannelClientManager deviceChannelManager, IFactory factory, AdbPacketSendWork adbServerSender, ChannelJobDispatcher dispatcher)
		{
			if (deviceChannelManager == null)
			{
				throw new ArgumentNullException("deviceChannelManager");
			}
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}
			if (adbServerSender == null)
			{
				throw new ArgumentNullException("adbServerSender");
			}
			this.deviceChannelManager = deviceChannelManager;
			this.factory = factory;
			this.dispatcher = dispatcher;
			this.adbServerSender = adbServerSender;
		}

		public bool HandlePacket(AdbPacket receivedPacket)
		{
			if (receivedPacket == null)
			{
				throw new ArgumentNullException("receivedPacket");
			}
			ShellAmStartParam shellAmStartParam = null;
			if (receivedPacket.Command == 1313165391)
			{
				string command = AdbPacket.ParseStringFromData(receivedPacket.Data);
				shellAmStartParam = ShellAmStartParam.ParseFromOpen(command);
			}
			else if (factory.AgentConfiguration.EnableInteractiveShell && receivedPacket.Command == 1163154007 && InteractiveShellChannels.ChannelExists(receivedPacket.Arg0, receivedPacket.Arg1))
			{
				string @string = Encoding.UTF8.GetString(receivedPacket.Data, 0, receivedPacket.Data.Length);
				shellAmStartParam = ShellAmStartParam.ParseFromInteractiveShell(@string);
			}
			if (shellAmStartParam != null)
			{
				StartNewActivityStart(receivedPacket.Arg0, shellAmStartParam);
				return true;
			}
			return false;
		}

		private void StartNewActivityStart(uint remoteId, ShellAmStartParam param)
		{
			ShellChannelJob job = new ActivityStartJob(factory, param);
			if (!dispatcher.ExecuteJob(job, adbServerSender, deviceChannelManager, remoteId))
			{
				EtwLogger.Instance.TooManyPendingChannelJobs();
				adbServerSender.EnqueueClse(0u, remoteId);
			}
		}
	}
}