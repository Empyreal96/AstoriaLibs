using System;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public class AdbFileSyncQuitPacket : AdbFileSyncPacket
	{
		public static async Task<AdbFileSyncQuitPacket> ReadBodyAsync(IStreamReader stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!(await AdbFileSyncPacket.ReadUnitAsync(stream)).HasValue)
			{
				return null;
			}
			return new AdbFileSyncQuitPacket();
		}

		public override async Task<bool> WriteAsync(IStreamWriter stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] packet = new byte[8];
			IntegerHelper.WriteUintToLittleEndianBytes(1414092113u, packet, 0);
			IntegerHelper.WriteUintToLittleEndianBytes(0u, packet, 4);
			return await stream.WriteAsync(packet, 0, packet.Length) == packet.Length;
		}
	}
}