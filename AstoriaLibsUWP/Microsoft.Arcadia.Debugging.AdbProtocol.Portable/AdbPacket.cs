using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
	{
[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop reports error on the structure defection in comments")]
public sealed class AdbPacket
{
	public enum CommandDef
	{
		None = 0,
		Cnxn = 1314410051,
		Open = 1313165391,
		Okay = 1497451343,
		Wrte = 1163154007,
		Clse = 1163086915,
		Auth = 1213486401,
		Sync = 1129208147
	}

	public const int HeaderBytes = 24;

	private const uint MagicMask = uint.MaxValue;

	public uint Command { get; private set; }

	public uint Arg0 { get; private set; }

	public uint Arg1 { get; private set; }

	public uint DataCrc { get; private set; }

	public uint Magic { get; private set; }

	[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "We want to return a buffer")]
	public byte[] Data { get; private set; }

	[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "We want to return a buffer")]
	public byte[] RawBits { get; private set; }

	public AdbPacket(uint command, uint arg0, uint arg1)
	{
		InitializeData(command, arg0, arg1, null);
	}

	public AdbPacket(uint command, uint arg0, uint arg1, byte[] data, int start, int bytes)
	{
		BufferHelper.CheckAccessRange(data, start, bytes);
		byte[] array = new byte[bytes];
		Array.Copy(data, start, array, 0, bytes);
		InitializeData(command, arg0, arg1, array);
	}

	public AdbPacket(uint command, uint arg0, uint arg1, byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			throw new ArgumentException("data must be provided", "data");
		}
		byte[] array = new byte[data.Length];
		Array.Copy(data, array, data.Length);
		InitializeData(command, arg0, arg1, array);
	}

	private AdbPacket()
	{
	}

	public static bool IsChannelPacket(AdbPacket packet)
	{
		if (packet == null)
		{
			throw new ArgumentNullException("packet");
		}
		if (packet.Command != 1313165391 && packet.Command != 1163154007 && packet.Command != 1497451343)
		{
			return packet.Command == 1163086915;
		}
		return true;
	}

	public static string ParseStringFromData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return null;
		}
		int num = data.Length;
		while (num > 0 && data[num - 1] == 0)
		{
			num--;
		}
		if (num == 0)
		{
			return null;
		}
		try
		{
			return Encoding.UTF8.GetString(data, 0, num);
		}
		catch (ArgumentException)
		{
			return null;
		}
	}

	public static uint ParseDataBytesFromHeaderBuffer(byte[] headerBuffer)
	{
		return IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 12);
	}

	public static AdbPacket Parse(byte[] headerBuffer, byte[] bodyBuffer)
	{
		if (headerBuffer == null)
		{
			throw new ArgumentNullException("headerBuffer");
		}
		if (headerBuffer.Length < 24)
		{
			throw new ArgumentException("Insufficient buffer", "headerBuffer");
		}
		uint command = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 0);
		uint arg = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 4);
		uint arg2 = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 8);
		uint num = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 12);
		uint dataCrc = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 16);
		uint magic = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 20);
		byte[] array = null;
		if (num != 0)
		{
			if (bodyBuffer == null || num > bodyBuffer.Length)
			{
				throw new ArgumentException("Body buffer size doesn't match with header", "bodyBuffer");
			}
			array = new byte[num];
			Array.Copy(bodyBuffer, array, (int)num);
		}
		AdbPacket adbPacket = new AdbPacket();
		adbPacket.Command = command;
		adbPacket.Arg0 = arg;
		adbPacket.Arg1 = arg2;
		adbPacket.DataCrc = dataCrc;
		adbPacket.Magic = magic;
		adbPacket.Data = array;
		adbPacket.RawBits = Pack(headerBuffer, array);
		return adbPacket;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Cmd:\t" + IntegerHelper.GetAsciiStringFromInteger(Command));
		stringBuilder.AppendLine("Arg0:\t" + Arg0);
		stringBuilder.AppendLine("Arg1:\t" + Arg1);
		if (Data != null)
		{
			stringBuilder.AppendLine("Len:\t" + Data.Length);
			if (Command == 1314410051 || Command == 1313165391)
			{
				string text = ParseStringFromData(Data);
				stringBuilder.AppendLine("Data:\t" + ((text != null) ? text : "<bad data>"));
			}
		}
		else
		{
			stringBuilder.AppendLine("Len:\t0");
		}
		return stringBuilder.ToString();
	}

	private static byte[] Pack(byte[] head, byte[] data)
	{
		int num = ((data != null) ? data.Length : 0);
		byte[] array = new byte[head.Length + num];
		Array.Copy(head, array, head.Length);
		if (num > 0)
		{
			Array.Copy(data, 0, array, head.Length, num);
		}
		return array;
	}

	private static uint CalculateCyclicRedundancyCheck(byte[] buffer)
	{
		uint num = 0u;
		foreach (byte b in buffer)
		{
			try
			{
				num = checked(num + b);
			}
			catch (OverflowException)
			{
			}
		}
		return num;
	}

	private void InitializeData(uint command, uint arg0, uint arg1, byte[] data)
	{
		Command = command;
		Arg0 = arg0;
		Arg1 = arg1;
		Magic = command ^ 0xFFFFFFFFu;
		if (data != null && data.Length > 0)
		{
			DataCrc = CalculateCyclicRedundancyCheck(data);
			Data = data;
		}
		else
		{
			DataCrc = 0u;
			Data = null;
		}
		RawBits = Pack(GenerateHeaderBits(), data);
	}

	private byte[] GenerateHeaderBits()
	{
		byte[] array = new byte[24];
		IntegerHelper.WriteUintToLittleEndianBytes(Command, array, 0);
		IntegerHelper.WriteUintToLittleEndianBytes(Arg0, array, 4);
		IntegerHelper.WriteUintToLittleEndianBytes(Arg1, array, 8);
		if (Data != null)
		{
			IntegerHelper.WriteUintToLittleEndianBytes((uint)Data.Length, array, 12);
			IntegerHelper.WriteUintToLittleEndianBytes(DataCrc, array, 16);
		}
		else
		{
			IntegerHelper.WriteUintToLittleEndianBytes(0u, array, 12);
			IntegerHelper.WriteUintToLittleEndianBytes(0u, array, 16);
		}
		IntegerHelper.WriteUintToLittleEndianBytes(Magic, array, 20);
		return array;
	}
}
}