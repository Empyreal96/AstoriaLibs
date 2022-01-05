using System;
using System.Collections.Generic;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	internal sealed class MemoryPipe
	{
		private object lockObject = new object();

		private IList<byte[]> dataPieces = new List<byte[]>();

		private int head;

		public void Write(byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

		public void Write(byte[] buffer, int start, int bytes)
		{
			BufferHelper.CheckAccessRange(buffer, start, bytes);
			byte[] array = new byte[bytes];
			Array.Copy(buffer, start, array, 0, bytes);
			lock (lockObject)
			{
				dataPieces.Add(array);
			}
		}

		public int Read(byte[] buffer, int startIndex, int bytesToRead)
		{
			BufferHelper.CheckAccessRange(buffer, startIndex, bytesToRead);
			int num = startIndex;
			int num2 = bytesToRead;
			lock (lockObject)
			{
				while (dataPieces.Count != 0)
				{
					int num3 = Math.Min(dataPieces[0].Length - head, num2);
					Array.Copy(dataPieces[0], head, buffer, num, num3);
					num += num3;
					num2 -= num3;
					head += num3;
					if (head == dataPieces[0].Length)
					{
						dataPieces.RemoveAt(0);
						head = 0;
					}
					if (num2 == 0)
					{
						break;
					}
				}
			}
			return bytesToRead - num2;
		}
	}
}
