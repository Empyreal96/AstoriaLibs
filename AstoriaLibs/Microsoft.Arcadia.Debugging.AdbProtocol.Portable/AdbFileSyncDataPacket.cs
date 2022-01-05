using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public class AdbFileSyncDataPacket : AdbFileSyncPacket
	{
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Return byte array is more natural in this case")]
		public byte[] Data { get; private set; }

		private AdbFileSyncDataPacket(byte[] data)
		{
			Data = data;
		}

		public static async Task<AdbFileSyncDataPacket> ReadBodyAsync(IStreamReader stream)
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
					byte[] data = await AdbFileSyncPacket.ReadBinaryAsync(stream, bytes.Value);
					if (data == null)
					{
						return null;
					}
					return new AdbFileSyncDataPacket(data);
				}
			}
			return null;
		}
	}
}