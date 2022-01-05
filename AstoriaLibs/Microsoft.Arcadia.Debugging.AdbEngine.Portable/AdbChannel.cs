using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{

	internal sealed class AdbChannel : IAdbChannel, IDisposable
	{
		private AdbStreamReader streamReader;

		private AdbStreamWriter streamWriter;

		private AdbPacketSendWork sender;

		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Easy for debug")]
		private string name;

		private uint localId;

		private uint remoteId;

		public uint LocalId => localId;

		public uint RemoteId => remoteId;

		IStreamReader IAdbChannel.StreamReader => streamReader;

		IStreamWriter IAdbChannel.StreamWriter => streamWriter;

		public AdbChannel(string name, uint localId, uint remoteId, int maxAdbPacketDataBytes, AdbPacketSendWork sender)
		{
			this.name = name;
			this.localId = localId;
			this.remoteId = remoteId;
			this.sender = sender;
			streamReader = new AdbStreamReader();
			streamWriter = new AdbStreamWriter(sender, localId, remoteId, maxAdbPacketDataBytes);
		}

		public void OnClosed()
		{
			streamReader.OnClose();
			streamWriter.OnClose();
		}

		public void OnDataReceived(byte[] data)
		{
			streamReader.OnDataReceived(data);
			sender.EnqueueOkay(localId, remoteId);
		}

		public void OnSendAcknowledge()
		{
			streamWriter.OnAcknowledged();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				streamReader.Dispose();
				streamWriter.Dispose();
			}
		}
	}
}
