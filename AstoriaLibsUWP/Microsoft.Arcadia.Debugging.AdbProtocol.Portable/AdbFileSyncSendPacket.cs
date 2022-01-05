using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public sealed class AdbFileSyncSendPacket : AdbFileSyncPacket
	{
		public string DeviceFilePath { get; private set; }

		public uint Permission { get; private set; }

		private AdbFileSyncSendPacket(string deviceFilePath, uint permission)
		{
			DeviceFilePath = deviceFilePath;
			Permission = permission;
		}

		public static async Task<AdbFileSyncSendPacket> ReadBodyAsync(IStreamReader stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			uint? bytes = await AdbFileSyncPacket.ReadUnitAsync(stream);
			if (bytes.HasValue)
			{
				uint? num = bytes;
				if (num.GetValueOrDefault() != 0 || !num.HasValue)
				{
					string data = await AdbFileSyncPacket.ReadStringFromUtf8Async(stream, bytes.Value);
					if (data == null)
					{
						return null;
					}
					string[] subStrings = data.Split(',');
					if (subStrings.Length != 2)
					{
						return null;
					}
					if (!uint.TryParse(subStrings[1], out var permission))
					{
						return null;
					}
					return new AdbFileSyncSendPacket(subStrings[0], permission);
				}
			}
			return null;
		}
	}
}