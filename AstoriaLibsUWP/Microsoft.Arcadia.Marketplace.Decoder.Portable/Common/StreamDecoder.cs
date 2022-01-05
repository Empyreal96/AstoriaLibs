using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
	public abstract class StreamDecoder : IDisposable
	{
		private uint offset;

		private long boundary;

		private Stack<long> boundaryStack;

		public long Boundary
		{
			get
			{
				return boundary;
			}
			private set
			{
				Interlocked.Exchange(ref boundary, value);
			}
		}

		public uint Offset
		{
			get
			{
				return offset;
			}
			set
			{
				if (value > FileStream.Length || value > Boundary)
				{
					throw new ApkDecoderCommonException("Attempted to navigate outside of allowed boundary");
				}
				offset = value;
			}
		}

		protected string FilePath { get; private set; }

		protected Stream FileStream { get; private set; }

		protected StreamDecoder(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException("File path must be provided", "filePath");
			}
			MemoryStream memoryStream = null;
			try
			{
				using (Stream stream = PortableUtilsServiceLocator.FileUtils.OpenReadOnlyFileStream(filePath))
				{ 
					memoryStream = new MemoryStream(Convert.ToInt32(stream.Length));
				stream.CopyTo(memoryStream);
				FilePath = filePath;
				FileStream = memoryStream;
				boundary = FileStream.Length;
				boundaryStack = new Stack<long>();
				}
			}
			catch
			{
				memoryStream?.Dispose();
				throw;
			}
		}

		public byte ReadByte()
		{
			byte[] array = ReadBytes(1u);
			Offset++;
			return array[0];
		}

		public ushort PeakUint16()
		{
			byte[] array = ReadBytes(2u);
			return (ushort)(((ushort)(array[1] << 8) & 0xFF00u) | (array[0] & 0xFFu));
		}

		public ushort ReadUint16()
		{
			ushort result = PeakUint16();
			Offset += 2u;
			return result;
		}

		public uint ReadUint32()
		{
			byte[] array = ReadBytes(4u);
			Offset += 4u;
			return ((uint)(array[3] << 24) & 0xFF000000u) | ((uint)(array[2] << 16) & 0xFF0000u) | ((uint)(array[1] << 8) & 0xFF00u) | (array[0] & 0xFFu);
		}

		public string ReadString(bool isUtf8)
		{
			return isUtf8 ? ReadAsUtf8() : ReadAsUtf16();
		}

		public void PushReadBoundary(long newBoundary)
		{
			if (newBoundary <= Offset)
			{
				throw new ApkDecoderCommonException("Boundary should be set with the value beyond the offset.");
			}
			if (newBoundary > FileStream.Length)
			{
				throw new ApkDecoderCommonException("Boundary should be set with the value smaller than the total data size");
			}
			boundaryStack.Push(Boundary);
			Boundary = newBoundary;
		}

		public void PopReadBoundary()
		{
			if (boundaryStack.Count == 0)
			{
				throw new ApkDecoderCommonException("Boundary stack is empty");
			}
			Boundary = boundaryStack.Pop();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && FileStream != null)
			{
				FileStream.Dispose();
			}
		}

		private string ReadAsUtf8()
		{
			uint num = ReadStringLength(isUtf8: true);
			uint num2 = ReadStringLength(isUtf8: true);
			byte[] array = new byte[num2];
			int num3 = 0;
			for (uint num4 = 0u; num4 < num2; num4++)
			{
				byte b = ReadByte();
				if (b == 0)
				{
					LoggerCore.Log("Early NUL character encountered.");
					break;
				}
				array[num4] = b;
				num3++;
			}
			string @string = Encoding.UTF8.GetString(array, 0, num3);
			if (@string.Length != num)
			{
				LoggerCore.Log("String Length mismatches. Probably it's truncated. Actual Length: {0}, Expected: {1}", @string.Length, num);
			}
			return @string;
		}

		private string ReadAsUtf16()
		{
			uint num = ReadStringLength(isUtf8: false);
			char[] array = new char[num];
			for (uint num2 = 0u; num2 < num; num2++)
			{
				ushort num3 = ReadUint16();
				if (num3 == 0)
				{
					throw new ApkDecoderCommonException("NULL is not expected");
				}
				array[num2] = (char)num3;
			}
			return new string(array);
		}

		private uint ReadStringLength(bool isUtf8)
		{
			if (isUtf8)
			{
				uint num = ReadByte();
				if ((num & 0x80u) != 0)
				{
					byte b = ReadByte();
					num = ((num & 0x7F) << 8) | b;
				}
				return num;
			}
			uint num2 = ReadUint16();
			if ((num2 & 0x8000u) != 0)
			{
				ushort num3 = ReadUint16();
				num2 = ((num2 & 0x7FFF) << 16) | num3;
			}
			return num2;
		}

		private byte[] ReadBytes(uint count)
		{
			ValidateRead(count);
			byte[] array = new byte[count];
			FileStream.Position = Offset;
			int num = FileStream.Read(array, 0, (int)count);
			if (num != (int)count)
			{
				throw new ApkDecoderCommonException("File read out of the boundary");
			}
			return array;
		}

		private void ValidateRead(uint length)
		{
			if (length == 0)
			{
				throw new ApkDecoderCommonException("Read length can't be 0");
			}
			uint num = checked(Offset + length);
			if (num > Boundary)
			{
				throw new ApkDecoderCommonException("Attempted to read outside of allowed boundary");
			}
		}
	}
}