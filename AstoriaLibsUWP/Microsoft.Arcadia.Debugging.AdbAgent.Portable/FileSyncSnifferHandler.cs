using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class FileSyncSnifferHandler : IAdbPacketHandler
	{
		private object lockObj = new object();

		private Dictionary<uint, FileSyncSnifferSink> snifferSessions = new Dictionary<uint, FileSyncSnifferSink>();

		private IFactory factory;

		public FileSyncSnifferHandler(IFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.factory = factory;
		}

		public bool HandlePacket(AdbPacket receivedPacket)
		{
			if (receivedPacket == null)
			{
				throw new ArgumentNullException("receivedPacket");
			}
			if (receivedPacket.Command == 1313165391)
			{
				HandleCommandOpen(receivedPacket);
			}
			else if (receivedPacket.Command == 1163154007)
			{
				HandleCommandWrite(receivedPacket);
			}
			else if (receivedPacket.Command == 1163086915)
			{
				HandleCommandClose(receivedPacket);
			}
			return false;
		}

		private void HandleCommandOpen(AdbPacket receivedPacket)
		{
			string text = AdbPacket.ParseStringFromData(receivedPacket.Data);
			if (text == "sync:")
			{
				HandleOpenSync(receivedPacket);
				return;
			}
			ShellRmParams shellRmParams = ShellRmParams.Parse(text);
			if (shellRmParams != null && shellRmParams.FilePath != null)
			{
				HandleOpenShellRm(shellRmParams);
			}
		}

		private void HandleOpenShellRm(ShellRmParams removeParams)
		{
			string linuxDirectoryName = IOUtils.GetLinuxDirectoryName(removeParams.FilePath);
			if (string.Compare(linuxDirectoryName, factory.AgentConfiguration.RemoteDataSnifferDirectory, StringComparison.Ordinal) != 0)
			{
				LoggerCore.Log("Not deleting {0} as the filePath is not on the white list.", linuxDirectoryName);
				return;
			}
			string localDataSniffedDirectory = factory.AgentConfiguration.LocalDataSniffedDirectory;
			string text = Path.Combine(new string[2]
			{
			localDataSniffedDirectory,
			Path.GetFileName(removeParams.FilePath)
			});
			PathSanitizer pathSanitizer = new PathSanitizer(text);
			if (!pathSanitizer.IsWithinFolder(localDataSniffedDirectory))
			{
				LoggerCore.Log("Possible directory escape attack on rmdir.");
				return;
			}
			LoggerCore.Log("Request to delete {0}'s local intercepted copy, {1}...", removeParams.FilePath, text);
			IOUtils.RemoveFile(text);
		}

		private void HandleCommandWrite(AdbPacket receivedPacket)
		{
			FileSyncSnifferSink fileSyncSnifferSink = null;
			lock (lockObj)
			{
				FindSnifferSession(receivedPacket.Arg0)?.OnData(receivedPacket.Data);
			}
		}

		private void HandleCommandClose(AdbPacket receivedPacket)
		{
			lock (lockObj)
			{
				FileSyncSnifferSink fileSyncSnifferSink = FindSnifferSession(receivedPacket.Arg0);
				if (fileSyncSnifferSink != null)
				{
					fileSyncSnifferSink.Stop();
					snifferSessions.Remove(fileSyncSnifferSink.RemoteId);
				}
			}
		}

		private void HandleOpenSync(AdbPacket receivedPacket)
		{
			lock (lockObj)
			{
				LoggerCore.Log("New SYNC detected");
				FileSyncSnifferSink fileSyncSnifferSink = null;
				try
				{
					LoggerCore.Log("Remote ID for SYNC Channel is: {0}.", receivedPacket.Arg0);
					fileSyncSnifferSink = new FileSyncSnifferSink(factory, receivedPacket.Arg0);
					fileSyncSnifferSink.OnTransferFinished += Channel_OnInterceptFinished;
					fileSyncSnifferSink.OnTransferFail += Channel_OnInterceptFinished;
					snifferSessions.Add(fileSyncSnifferSink.RemoteId, fileSyncSnifferSink);
					fileSyncSnifferSink.Start();
					fileSyncSnifferSink = null;
				}
				finally
				{
					fileSyncSnifferSink?.Dispose();
				}
			}
		}

		private FileSyncSnifferSink FindSnifferSession(uint remoteId)
		{
			FileSyncSnifferSink value = null;
			if (snifferSessions.TryGetValue(remoteId, out value))
			{
				return value;
			}
			return null;
		}

		private void Channel_OnInterceptFinished(FileSyncSnifferSink channel)
		{
			lock (lockObj)
			{
				if (snifferSessions.ContainsValue(channel))
				{
					snifferSessions.Remove(channel.RemoteId);
				}
				else
				{
					LoggerCore.Log("Sniffer session does not exist in the set.");
				}
			}
		}
	}
}