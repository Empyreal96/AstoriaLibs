using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public abstract class AdbFileSyncPacket
	{
		public enum Direction
		{
			FromServer,
			FromClient
		}

		protected enum FileSyncCommand
		{
			None = 0,
			Data = 1096040772,
			Dent = 1414415684,
			Done = 1162760004,
			Fail = 1279869254,
			List = 1414744396,
			Okay = 1497451343,
			Quit = 1414092113,
			Recv = 1447249234,
			Send = 1145980243,
			Stat = 1413567571
		}

		public static async Task<AdbFileSyncPacket> ReadAsync(IStreamReader stream, Direction direction)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] buffer = new byte[4];
			if (await stream.ReadAsync(buffer, 0, buffer.Length) != buffer.Length)
			{
				return null;
			}
			switch (IntegerHelper.Read32BitValueFromLittleEndianBytes(buffer, 0))
			{
				case 1145980243u:
					return await AdbFileSyncSendPacket.ReadBodyAsync(stream);
				case 1447249234u:
					return await AdbFileSyncReceivePacket.ReadBodyAsync(stream);
				case 1096040772u:
					return await AdbFileSyncDataPacket.ReadBodyAsync(stream);
				case 1414744396u:
					return await AdbFileSyncListPacket.ReadBodyAsync(stream);
				case 1279869254u:
					return await AdbFileSyncFailPacket.ReadBodyAsync(stream);
				case 1413567571u:
					if (direction == Direction.FromClient)
					{
						return await AdbFileSyncStatPacketFromClient.ReadBodyAsync(stream);
					}
					return await AdbFileSyncStatPacketFromServer.ReadBodyAsync(stream);
				case 1414415684u:
					return await AdbFileSyncDirectoryEntryPacket.ReadBodyAsync(stream);
				case 1162760004u:
					return await AdbFileSyncDonePacket.ReadBodyAsync(stream);
				case 1497451343u:
					return await AdbFileSyncOkayPacket.ReadBodyAsync(stream);
				case 1414092113u:
					return await AdbFileSyncQuitPacket.ReadBodyAsync(stream);
				default:
					return null;
			}
		}

		public virtual Task<bool> WriteAsync(IStreamWriter stream)
		{
			throw new NotSupportedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is more readable")]
		protected static async Task<uint?> ReadUnitAsync(IStreamReader stream)
		{
			byte[] buffer = new byte[4];
			int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
			if (bytesRead != buffer.Length)
			{
				return null;
			}
			return IntegerHelper.Read32BitValueFromLittleEndianBytes(buffer, 0);
		}

		protected static async Task<byte[]> ReadBinaryAsync(IStreamReader stream, uint bytesToRead)
		{
			byte[] buffer = new byte[bytesToRead];
			int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
			if (bytesRead != buffer.Length)
			{
				return null;
			}
			return buffer;
		}

		protected static async Task<string> ReadStringFromUtf8Async(IStreamReader stream, uint bytesToRead)
		{
			byte[] buffer = await ReadBinaryAsync(stream, bytesToRead);
			if (buffer == null)
			{
				return null;
			}
			try
			{
				return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
			}
			catch
			{
				return null;
			}
		}
	}
}