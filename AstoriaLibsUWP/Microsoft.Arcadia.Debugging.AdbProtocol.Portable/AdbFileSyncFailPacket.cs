using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public sealed class AdbFileSyncFailPacket : AdbFileSyncPacket
	{
		public string ErrorMessage { get; private set; }

		private AdbFileSyncFailPacket(string error)
		{
			ErrorMessage = error;
		}

		public static async Task<AdbFileSyncFailPacket> ReadBodyAsync(IStreamReader stream)
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
					return new AdbFileSyncFailPacket(data);
				}
			}
			return null;
		}
	}
}