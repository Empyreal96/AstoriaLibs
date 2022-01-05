using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public sealed class AdbFileSyncListPacket : AdbFileSyncPacket
	{
		public string DeviceFolderPath { get; private set; }

		private AdbFileSyncListPacket(string deviceFolderPath)
		{
			DeviceFolderPath = deviceFolderPath;
		}

		public static async Task<AdbFileSyncListPacket> ReadBodyAsync(IStreamReader stream)
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
					return new AdbFileSyncListPacket(data);
				}
			}
			return null;
		}
	}
}