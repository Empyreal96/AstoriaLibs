using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public sealed class AdbFileSyncDonePacket : AdbFileSyncPacket
	{
		public uint LastModifiedTime { get; private set; }

		private AdbFileSyncDonePacket(uint lastModifiedTime)
		{
			LastModifiedTime = lastModifiedTime;
		}

		public static async Task<AdbFileSyncDonePacket> ReadBodyAsync(IStreamReader stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			uint? time = await AdbFileSyncPacket.ReadUnitAsync(stream);
			if (!time.HasValue)
			{
				return null;
			}
			return new AdbFileSyncDonePacket(time.Value);
		}
	}
}