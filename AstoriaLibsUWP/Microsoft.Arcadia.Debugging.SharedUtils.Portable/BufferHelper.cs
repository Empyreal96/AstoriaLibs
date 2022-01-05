using System;

namespace Microsoft.Arcadia.Debugging.SharedUtils.Portable
{
	public static class BufferHelper
	{
		public static void CheckAccessRange(byte[] buffer, int startIndex, int bytesToAccess)
		{
			if (buffer == null || buffer.Length <= 0)
			{
				throw new ArgumentException("buffer must be provided", "buffer");
			}
			if (startIndex < 0 || startIndex >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (bytesToAccess <= 0 || bytesToAccess > buffer.Length - startIndex)
			{
				throw new ArgumentOutOfRangeException("bytesToAccess");
			}
		}
	}
}


