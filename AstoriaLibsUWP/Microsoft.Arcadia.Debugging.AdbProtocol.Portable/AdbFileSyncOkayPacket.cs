using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public sealed class AdbFileSyncOkayPacket : AdbFileSyncPacket
	{
		private AdbFileSyncOkayPacket()
		{
		}

		public static async Task<AdbFileSyncOkayPacket> ReadBodyAsync(IStreamReader stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!(await AdbFileSyncPacket.ReadUnitAsync(stream)).HasValue)
			{
				return null;
			}
			return new AdbFileSyncOkayPacket();
		}
	}
}