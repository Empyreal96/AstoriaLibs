using System;
using System.Text;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable;

internal class ApkUninstallHandler : IAdbPacketHandler
{
	private IAdbChannelClientManager channelManager;

	private IFactory factory;

	private AdbPacketSendWork adbServerSender;

	private ChannelJobDispatcher dispatcher;

	public ApkUninstallHandler(IAdbChannelClientManager deviceChannelManager, IFactory factory, AdbPacketSendWork adbServerSender, ChannelJobDispatcher dispatcher)
	{
		if (deviceChannelManager == null)
		{
			throw new ArgumentNullException("deviceChannelManager");
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
		channelManager = deviceChannelManager;
		this.adbServerSender = adbServerSender;
		this.dispatcher = dispatcher;
	}

	bool IAdbPacketHandler.HandlePacket(AdbPacket receivedPacket)
	{
		if (receivedPacket == null)
		{
			throw new ArgumentNullException("receivedPacket");
		}
		ShellPmUninstallParam shellPmUninstallParam = null;
		if (receivedPacket.Command == 1313165391)
		{
			string command = AdbPacket.ParseStringFromData(receivedPacket.Data);
			shellPmUninstallParam = ShellPmUninstallParam.ParseFromOpen(command);
		}
		else if (factory.AgentConfiguration.EnableInteractiveShell && receivedPacket.Command == 1163154007 && InteractiveShellChannels.ChannelExists(receivedPacket.Arg0, receivedPacket.Arg1))
		{
			string @string = Encoding.UTF8.GetString(receivedPacket.Data, 0, receivedPacket.Data.Length);
			shellPmUninstallParam = ShellPmUninstallParam.ParseFromInteractiveShell(@string);
		}
		if (shellPmUninstallParam != null)
		{
			StartNewUninstallJob(receivedPacket, shellPmUninstallParam);
			return true;
		}
		return false;
	}

	private void StartNewUninstallJob(AdbPacket receivedPacket, ShellPmUninstallParam param)
	{
		ApkUninstallJob job = new ApkUninstallJob(factory, param);
		dispatcher.ExecuteJob(job, adbServerSender, channelManager, receivedPacket.Arg0, receivedPacket.Arg1);
	}
}
