using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public class AdbFileSyncStatPacketFromServer : AdbFileSyncPacket
	{
		public uint Mode { get; private set; }

		public uint Size { get; private set; }

		public uint LastModifiedTime { get; private set; }

		private AdbFileSyncStatPacketFromServer(uint mode, uint size, uint lastModifedTime)
		{
			Mode = mode;
			Size = size;
			LastModifiedTime = lastModifedTime;
		}

		public static async Task<AdbFileSyncStatPacketFromServer> ReadBodyAsync(IStreamReader stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			uint? mode = await AdbFileSyncPacket.ReadUnitAsync(stream);
			if (!mode.HasValue)
			{
				return null;
			}
			uint? size = await AdbFileSyncPacket.ReadUnitAsync(stream);
			if (!size.HasValue)
			{
				return null;
			}
			uint? time = await AdbFileSyncPacket.ReadUnitAsync(stream);
			if (!time.HasValue)
			{
				return null;
			}
			return new AdbFileSyncStatPacketFromServer(mode.Value, size.Value, time.Value);
		}
	}
}