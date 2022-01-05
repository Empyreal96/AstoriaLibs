using System;
using System.Threading;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public sealed class AdbPacketReceivWork : IWork
	{
		private enum State
		{
			ReadingHeader,
			ReadingBody
		}

		private const int MaximumPermittedBufferSize = 1048576;

		private ISocketReceiveWork receiveWork;

		private MemoryPipe pipe = new MemoryPipe();

		private byte[] packetHeader = new byte[24];

		private int packetHeaderCursor;

		private int maxPacketBytes = 4096;

		private byte[] packetBody;

		private int packetBodyCursor;

		private State state;

		WaitHandle IWork.SignalHandle => receiveWork.SignalHandle;

		public int MaxPacketBytes
		{
			get
			{
				return maxPacketBytes;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value", "Must be a positive natural number.");
				}
				if (value > 1048576)
				{
					throw new ArgumentOutOfRangeException("value", "Value is too large.");
				}
				maxPacketBytes = value;
			}
		}

		public event EventHandler<AdbPacketReceivedEventArgs> AdbPacketReceived;

		public AdbPacketReceivWork(ISocketReceiveWork receiveWork)
		{
			if (receiveWork == null)
			{
				throw new ArgumentNullException("receiveWork");
			}
			this.receiveWork = receiveWork;
			this.receiveWork.DataReceived += OnDataReceived;
		}

		void IWork.DoWork()
		{
			receiveWork.DoWork();
		}

		private void OnDataReceived(object sender, SocketDataReceivedEventArgs args)
		{
			pipe.Write(args.Data);
			int num5;
			while (true)
			{
				if (state == State.ReadingHeader)
				{
					int num = packetHeader.Length - packetHeaderCursor;
					int num2 = pipe.Read(packetHeader, packetHeaderCursor, num);
					if (num2 != num)
					{
						packetHeaderCursor += num2;
						return;
					}
					uint num3 = AdbPacket.ParseDataBytesFromHeaderBuffer(packetHeader);
					if (num3 > MaxPacketBytes)
					{
						throw new InvalidOperationException("Allocated buffer size would be over agreed maximum size.");
					}
					if (num3 != 0)
					{
						state = State.ReadingBody;
						packetBody = new byte[num3];
						packetBodyCursor = 0;
					}
					else
					{
						NotifyAdbPacketReceived(packetHeader, null);
						packetHeaderCursor = 0;
					}
				}
				else
				{
					int num4 = packetBody.Length - packetBodyCursor;
					num5 = pipe.Read(packetBody, packetBodyCursor, num4);
					if (num5 != num4)
					{
						break;
					}
					NotifyAdbPacketReceived(packetHeader, packetBody);
					state = State.ReadingHeader;
					packetHeaderCursor = 0;
				}
			}
			packetBodyCursor += num5;
		}

		private void NotifyAdbPacketReceived(byte[] head, byte[] body)
		{
			if (this.AdbPacketReceived != null)
			{
				AdbPacket packet = AdbPacket.Parse(head, body);
				this.AdbPacketReceived(this, new AdbPacketReceivedEventArgs(packet));
			}
		}
	}
}
