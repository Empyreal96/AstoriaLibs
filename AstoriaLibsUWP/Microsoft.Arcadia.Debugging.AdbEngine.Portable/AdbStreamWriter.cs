using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	internal sealed class AdbStreamWriter : IStreamWriter, IDisposable
	{
		private AdbPacketSendWork sender;

		private uint localId;

		private uint remoteId;

		private int maxAdbPacketDataBytes;

		private ManualResetEvent closeEvent = new ManualResetEvent(initialState: false);

		private AutoResetEvent ackReceivedEvent = new AutoResetEvent(initialState: false);

		public AdbStreamWriter(AdbPacketSendWork sender, uint localId, uint remoteId, int maxAdbPacketDataBytes)
		{
			this.sender = sender;
			this.localId = localId;
			this.remoteId = remoteId;
			this.maxAdbPacketDataBytes = maxAdbPacketDataBytes;
		}

		public void OnAcknowledged()
		{
			ackReceivedEvent.Set();
		}

		public void OnClose()
		{
			closeEvent.Set();
		}

		async Task<int> IStreamWriter.WriteAsync(byte[] buffer, int startIndex, int bytesToWrite)
		{
			return await Task.Run(() => Write(buffer, startIndex, bytesToWrite));
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
				closeEvent.Dispose();
				ackReceivedEvent.Dispose();
			}
		}

		private int Write(byte[] buffer, int startIndex, int bytesToWrite)
		{
			BufferHelper.CheckAccessRange(buffer, startIndex, bytesToWrite);
			int num = startIndex;
			int num2 = bytesToWrite;
			while (true)
			{
				int num3 = Math.Min(num2, maxAdbPacketDataBytes);
				sender.EnqueueWrte(localId, remoteId, buffer, num, num3);
				num += num3;
				num2 -= num3;
				if (num2 == 0)
				{
					break;
				}
				WaitHandle[] waitHandles = new WaitHandle[2] { closeEvent, ackReceivedEvent };
				try
				{
					if (WaitHandle.WaitAny(waitHandles) == 0)
					{
						break;
					}
				}
				catch (ObjectDisposedException)
				{
					break;
				}
			}
			return bytesToWrite - num2;
		}
	}
}
