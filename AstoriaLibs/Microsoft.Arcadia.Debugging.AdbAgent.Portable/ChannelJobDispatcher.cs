using System;
using System.Collections.Generic;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ChannelJobDispatcher
	{
		private const int LocalIdBase = 2097152;

		private const int MaxJobCount = 1024;

		private object lockObject = new object();

		private IList<ChannelJob> jobs = new List<ChannelJob>();

		public bool ExecuteJob(ChannelJob job, AdbPacketSendWork adbServerSender, IAdbChannelClientManager channelManager, uint remoteId)
		{
			return ExecuteJob(job, adbServerSender, channelManager, remoteId, 0u);
		}

		public bool ExecuteJob(ChannelJob job, AdbPacketSendWork adbServerSender, IAdbChannelClientManager channelManager, uint remoteId, uint localId)
		{
			if (job == null)
			{
				throw new ArgumentNullException("job");
			}
			if (adbServerSender == null)
			{
				throw new ArgumentNullException("adbServerSender");
			}
			if (channelManager == null)
			{
				throw new ArgumentNullException("channelManager");
			}
			if (remoteId == 0)
			{
				throw new ArgumentException("ID should not be zero", "remoteId");
			}
			lock (lockObject)
			{
				uint num = 0u;
				if (localId == 0)
				{
					uint? num2 = FindLocalId();
					if (!num2.HasValue)
					{
						return false;
					}
					num = num2.Value;
				}
				else
				{
					num = localId;
				}
				ChannelJobConfiguration channelJobConfiguration = new ChannelJobConfiguration();
				channelJobConfiguration.AdbServerSender = adbServerSender;
				channelJobConfiguration.RemoteChannelManager = channelManager;
				channelJobConfiguration.LocalId = num;
				channelJobConfiguration.RemoteId = remoteId;
				ChannelJobConfiguration configuration = channelJobConfiguration;
				jobs.Add(job);
				job.ExecuteAsync(configuration).ContinueWith(delegate
				{
					lock (lockObject)
					{
						jobs.Remove(job);
					}
				});
			}
			return true;
		}

		private uint? FindLocalId()
		{
			for (uint num = 2097152u; num < 2098176; num++)
			{
				bool flag = false;
				foreach (ChannelJob job in jobs)
				{
					if (num == job.Configuration.LocalId)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return num;
				}
			}
			return null;
		}
	}
}