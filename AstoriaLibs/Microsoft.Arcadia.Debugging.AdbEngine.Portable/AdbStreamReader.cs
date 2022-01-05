using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public sealed class AdbStreamReader : IStreamReader, IDisposable
	{
		private ManualResetEvent closeEvent = new ManualResetEvent(initialState: false);

		private AutoResetEvent dataReceivedEvent = new AutoResetEvent(initialState: false);

		private MemoryPipe pipe = new MemoryPipe();

		public void OnDataReceived(byte[] data)
		{
			pipe.Write(data);
			dataReceivedEvent.Set();
		}

		public void OnClose()
		{
			closeEvent.Set();
		}

		async Task<int> IStreamReader.ReadAsync(byte[] buffer, int startIndex, int bytesToRead)
		{
			return await Task.Run(() => Read(buffer, startIndex, bytesToRead));
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
				dataReceivedEvent.Dispose();
			}
		}

		private int Read(byte[] buffer, int startIndex, int bytesToRead)
		{
			int num = startIndex;
			int num2 = bytesToRead;
			while (true)
			{
				int num3 = pipe.Read(buffer, num, num2);
				num += num3;
				num2 -= num3;
				if (num2 == 0)
				{
					break;
				}
				WaitHandle[] waitHandles = new WaitHandle[2] { closeEvent, dataReceivedEvent };
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
			return bytesToRead - num2;
		}
	}
}
