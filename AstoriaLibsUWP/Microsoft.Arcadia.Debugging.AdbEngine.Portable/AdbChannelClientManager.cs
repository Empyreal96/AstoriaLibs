using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{

	public sealed class AdbChannelClientManager : IAdbPacketHandler, IAdbChannelClientManager
	{
		private class OpenPendingInfo : IDisposable
		{
			private ManualResetEvent finishedEvent = new ManualResetEvent(initialState: false);

			public string Name { get; private set; }

			public uint LocalId { get; private set; }

			public uint? RemoteId { get; private set; }

			public OpenPendingInfo(string name, uint localId)
			{
				Name = name;
				LocalId = localId;
				RemoteId = null;
			}

			public void MarkOpenSucceeded(uint remoteId)
			{
				RemoteId = remoteId;
				finishedEvent.Set();
			}

			public void MarkOpenFailed()
			{
				finishedEvent.Set();
			}

			public void Wait()
			{
				finishedEvent.WaitOne();
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
					finishedEvent.Dispose();
				}
			}
		}

		private const int LocalIdBase = 1048576;

		private const int MaxChannelCount = 1024;

		private object lockObject = new object();

		private AdbPacketSendWork senderToAdbd;

		private int maxPacketDataBytes;

		private IList<OpenPendingInfo> pendings = new List<OpenPendingInfo>();

		private IList<AdbChannel> channels = new List<AdbChannel>();

		public AdbChannelClientManager(AdbPacketSendWork senderToAdbd, int maxPacketDataBytes)
		{
			this.senderToAdbd = senderToAdbd;
			this.maxPacketDataBytes = maxPacketDataBytes;
		}

		bool IAdbPacketHandler.HandlePacket(AdbPacket receivedPacket)
		{
			if (receivedPacket == null)
			{
				throw new ArgumentNullException("receivedPacket");
			}
			if (!AdbPacket.IsChannelPacket(receivedPacket))
			{
				return false;
			}
			lock (lockObject)
			{
				OpenPendingInfo openPendingInfo = null;
				foreach (OpenPendingInfo pending in pendings)
				{
					if (pending.LocalId == receivedPacket.Arg1)
					{
						openPendingInfo = pending;
						break;
					}
				}
				if (openPendingInfo != null)
				{
					if (receivedPacket.Command == 1497451343)
					{
						openPendingInfo.MarkOpenSucceeded(receivedPacket.Arg0);
					}
					else
					{
						openPendingInfo.MarkOpenFailed();
					}
					return true;
				}
				AdbChannel adbChannel = null;
				foreach (AdbChannel channel in channels)
				{
					if (channel.LocalId == receivedPacket.Arg1)
					{
						adbChannel = channel;
						break;
					}
				}
				if (adbChannel != null)
				{
					if (receivedPacket.Command == 1163086915)
					{
						adbChannel.OnClosed();
						channels.Remove(adbChannel);
					}
					else if (receivedPacket.Command == 1163154007)
					{
						adbChannel.OnDataReceived(receivedPacket.Data);
					}
					else if (receivedPacket.Command == 1497451343)
					{
						adbChannel.OnSendAcknowledge();
					}
					return true;
				}
			}
			return false;
		}

		async Task<IAdbChannel> IAdbChannelClientManager.OpenChannelAsync(string name)
		{
			return await Task.Run(() => OpenChannel(name));
		}

		void IAdbChannelClientManager.CloseChannel(IAdbChannel channel)
		{
			if (channel == null)
			{
				throw new ArgumentNullException("channel");
			}
			if (!(channel is AdbChannel adbChannel))
			{
				throw new ArgumentException("Input parameter is not of an expected type", "channel");
			}
			lock (lockObject)
			{
				if (channels.Remove(adbChannel))
				{
					senderToAdbd.EnqueueClse(adbChannel.LocalId, adbChannel.RemoteId);
					adbChannel.OnClosed();
				}
			}
		}

		private IAdbChannel OpenChannel(string name)
		{
			OpenPendingInfo openPendingInfo;
			lock (lockObject)
			{
				uint? num = FindLocalId();
				if (!num.HasValue)
				{
					throw new AdbEngineTooManyChannelsException();
				}
				openPendingInfo = new OpenPendingInfo(name, num.Value);
				pendings.Add(openPendingInfo);
			}
			senderToAdbd.EnqueueOpen(openPendingInfo.LocalId, name);
			openPendingInfo.Wait();
			if (!openPendingInfo.RemoteId.HasValue)
			{
				EtwLogger.Instance.OpenDaemonChannelFailure(openPendingInfo.LocalId, name);
				return null;
			}
			EtwLogger.Instance.OpenedDaemonChannel(openPendingInfo.LocalId, name);
			AdbChannel adbChannel = new AdbChannel(openPendingInfo.Name, openPendingInfo.LocalId, openPendingInfo.RemoteId.Value, maxPacketDataBytes, senderToAdbd);
			try
			{
				lock (lockObject)
				{
					channels.Add(adbChannel);
					pendings.Remove(openPendingInfo);
					AdbChannel result = adbChannel;
					adbChannel = null;
					return result;
				}
			}
			finally
			{
				adbChannel?.Dispose();
			}
		}

		private uint? FindLocalId()
		{
			lock (lockObject)
			{
				for (uint num = 1048576u; num < 1049600; num++)
				{
					bool flag = true;
					foreach (AdbChannel channel in channels)
					{
						if (channel.LocalId == num)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						foreach (OpenPendingInfo pending in pendings)
						{
							if (pending.LocalId == num)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						return num;
					}
				}
				return null;
			}
		}
	}
}
