using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public class AdbFileSyncDirectoryEntryPacket : AdbFileSyncPacket
	{
		public uint Mode { get; private set; }

		public uint Size { get; private set; }

		public uint LastModifiedTime { get; private set; }

		public string DeviceDirectoryName { get; private set; }

		private AdbFileSyncDirectoryEntryPacket(uint mode, uint size, uint lastModifiedTime, string deviceDirectoryName)
		{
			Mode = mode;
			Size = size;
			LastModifiedTime = lastModifiedTime;
			DeviceDirectoryName = deviceDirectoryName;
		}

		public static async Task<AdbFileSyncDirectoryEntryPacket> ReadBodyAsync(IStreamReader stream)
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
			uint? bytes = await AdbFileSyncPacket.ReadUnitAsync(stream);
			if (bytes.HasValue)
			{
				uint? num = bytes;
				if (num.GetValueOrDefault() != 0 || !num.HasValue)
				{
					string name = await AdbFileSyncPacket.ReadStringFromUtf8Async(stream, bytes.Value);
					if (name == null)
					{
						return null;
					}
					return new AdbFileSyncDirectoryEntryPacket(mode.Value, size.Value, time.Value, name);
				}
			}
			return null;
		}
	}
}