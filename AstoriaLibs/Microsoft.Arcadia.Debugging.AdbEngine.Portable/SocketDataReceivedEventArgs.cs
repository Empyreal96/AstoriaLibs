using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public sealed class SocketDataReceivedEventArgs : EventArgs
	{
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Array property is more appropriate in this case")]
		public byte[] Data { get; private set; }

		public SocketDataReceivedEventArgs(byte[] buffer, int start, int bytes)
		{
			BufferHelper.CheckAccessRange(buffer, start, bytes);
			byte[] array = new byte[bytes];
			Array.Copy(buffer, start, array, 0, bytes);
			Data = array;
		}

		public SocketDataReceivedEventArgs(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				throw new ArgumentException("data array should not be null or empty", "data");
			}
			Data = data;
		}
	}
}
