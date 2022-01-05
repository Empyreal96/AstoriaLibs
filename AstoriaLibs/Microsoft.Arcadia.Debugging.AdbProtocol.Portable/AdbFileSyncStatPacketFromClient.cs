using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public class AdbFileSyncStatPacketFromClient : AdbFileSyncPacket
	{
		public string DeviceFilePath { get; private set; }

		public AdbFileSyncStatPacketFromClient(string deviceFilePath)
		{
			DeviceFilePath = deviceFilePath;
		}

		public static async Task<AdbFileSyncStatPacketFromClient> ReadBodyAsync(IStreamReader stream)
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
					return new AdbFileSyncStatPacketFromClient(data);
				}
			}
			return null;
		}

		public override async Task<bool> WriteAsync(IStreamWriter stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] data = Encoding.UTF8.GetBytes(DeviceFilePath);
			byte[] packet = new byte[8 + data.Length];
			IntegerHelper.WriteUintToLittleEndianBytes(1413567571u, packet, 0);
			IntegerHelper.WriteUintToLittleEndianBytes((uint)data.Length, packet, 4);
			data.CopyTo(packet, 8);
			return await stream.WriteAsync(packet, 0, packet.Length) == packet.Length;
		}
	}
}