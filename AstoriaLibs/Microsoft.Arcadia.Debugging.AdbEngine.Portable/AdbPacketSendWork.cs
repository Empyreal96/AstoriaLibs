using System;
using System.Text;
using System.Threading;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public sealed class AdbPacketSendWork : IWork
	{
		private ISocketSendWork socketSendWork;

		WaitHandle IWork.SignalHandle => socketSendWork.SignalHandle;

		public AdbPacketSendWork(ISocketSendWork socketSendWork)
		{
			if (socketSendWork == null)
			{
				throw new ArgumentNullException("socketSendWork");
			}
			this.socketSendWork = socketSendWork;
		}

		void IWork.DoWork()
		{
			socketSendWork.DoWork();
		}

		public void EnqueueOpen(uint localId, string name)
		{
			AdbPacket packet = new AdbPacket(1313165391u, localId, 0u, Encoding.UTF8.GetBytes(name));
			EnqueueForSending(packet);
		}

		public void EnqueueOkay(uint localId, uint remoteId)
		{
			AdbPacket packet = new AdbPacket(1497451343u, localId, remoteId);
			EnqueueForSending(packet);
		}

		public void EnqueueClse(uint localId, uint remoteId)
		{
			AdbPacket packet = new AdbPacket(1163086915u, localId, remoteId);
			EnqueueForSending(packet);
		}

		public void EnqueueWrte(uint localId, uint remoteId, byte[] buffer, int startIndex, int length)
		{
			AdbPacket packet = new AdbPacket(1163154007u, localId, remoteId, buffer, startIndex, length);
			EnqueueForSending(packet);
		}

		public void EnqueueForSending(AdbPacket packet)
		{
			if (packet == null)
			{
				throw new ArgumentNullException("packet");
			}
			socketSendWork.EnqueueForSend(packet.RawBits);
		}
	}
}
