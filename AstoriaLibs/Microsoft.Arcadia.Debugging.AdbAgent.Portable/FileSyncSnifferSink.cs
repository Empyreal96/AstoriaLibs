using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class FileSyncSnifferSink : IDisposable
	{
		private Stream fileStream;

		private IFactory factory;

		private bool isRunning;

		private bool hasDisposed;

		private AdbStreamReader reader;

		private bool receivedSendPacket;

		public uint RemoteId { get; private set; }

		public string LocalFilePath { get; private set; }

		public string RemoteFilePath { get; private set; }

		public bool IsIntercepting { get; private set; }

		public event Action<FileSyncSnifferSink> OnTransferFinished;

		public event Action<FileSyncSnifferSink> OnTransferFail;

		public FileSyncSnifferSink(IFactory factory, uint remoteId)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this.factory = factory;
			RemoteId = remoteId;
		}

		public void Start()
		{
			if (isRunning)
			{
				throw new InvalidOperationException("Already running.");
			}
			isRunning = true;
			if (reader != null)
			{
				reader.Dispose();
			}
			reader = new AdbStreamReader();
			InterceptLoop().ContinueWith(delegate
			{
				LoggerCore.Log("Interception loop has completed.");
			});
		}

		public void Stop()
		{
			if (isRunning)
			{
				HandleClose(fireEvent: false, success: false);
			}
		}

		public void OnData(byte[] buffer)
		{
			if (isRunning)
			{
				reader.OnDataReceived(buffer);
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private async Task InterceptLoop()
		{
			while (isRunning)
			{
				AdbFileSyncPacket packet = await AdbFileSyncPacket.ReadAsync(reader, AdbFileSyncPacket.Direction.FromClient);
				if (packet is AdbFileSyncSendPacket sendPacket)
				{
					if (HandleSyncSendPacket(sendPacket))
					{
						receivedSendPacket = true;
						continue;
					}
					break;
				}
				if (packet is AdbFileSyncFailPacket)
				{
					HandleClose(fireEvent: true, success: false);
					break;
				}
				if (packet is AdbFileSyncDonePacket)
				{
					HandleClose(fireEvent: true, success: true);
					break;
				}
				if (packet is AdbFileSyncDataPacket dataPacket && receivedSendPacket)
				{
					await HandleSycDataPacket(dataPacket);
				}
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !hasDisposed)
			{
				if (fileStream != null)
				{
					fileStream.Dispose();
				}
				if (reader != null)
				{
					reader.Dispose();
				}
				hasDisposed = true;
			}
		}

		private bool HandleSyncSendPacket(AdbFileSyncSendPacket packet)
		{
			string linuxDirectoryName = IOUtils.GetLinuxDirectoryName(packet.DeviceFilePath);
			if (linuxDirectoryName != factory.AgentConfiguration.RemoteDataSnifferDirectory)
			{
				LoggerCore.Log("Not intercepting {0} as the filePath is not on the whitelist.", linuxDirectoryName);
				return false;
			}
			string localDataSniffedDirectory = factory.AgentConfiguration.LocalDataSniffedDirectory;
			string fileName = Path.GetFileName(packet.DeviceFilePath);
			if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				LoggerCore.Log("Invalid filename in interception filePath.");
				return false;
			}
			PathSanitizer pathSanitizer = new PathSanitizer(Path.Combine(new string[2] { localDataSniffedDirectory, fileName }));
			if (!pathSanitizer.IsWithinFolder(localDataSniffedDirectory))
			{
				LoggerCore.Log("Possible directory escape attack.");
				return false;
			}
			if (OpenLocalInterceptFile(localDataSniffedDirectory, pathSanitizer.Path))
			{
				LocalFilePath = pathSanitizer.Path;
				RemoteFilePath = packet.DeviceFilePath;
				LoggerCore.Log("Intercepting file SYNC {0} to {1}.", RemoteFilePath, LocalFilePath);
				return true;
			}
			LoggerCore.Log("Could not open cache file. Likely file in use.");
			return false;
		}

		private bool OpenLocalInterceptFile(string interceptLocalDirectory, string interceptFinalLocalPath)
		{
			try
			{
				if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(interceptLocalDirectory))
				{
					PortableUtilsServiceLocator.FileUtils.CreateDirectory(interceptLocalDirectory);
				}
				fileStream = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(interceptFinalLocalPath);
				IsIntercepting = true;
				return true;
			}
			catch (Exception exp)
			{
				if (ExceptionUtils.IsIOException(exp))
				{
					HandleClose(fireEvent: true, success: false);
					return false;
				}
				throw;
			}
		}

		private async Task HandleSycDataPacket(AdbFileSyncDataPacket packet)
		{
			await fileStream.WriteAsync(packet.Data, 0, packet.Data.Length);
		}

		private void HandleClose(bool fireEvent, bool success)
		{
			isRunning = false;
			IsIntercepting = false;
			if (success)
			{
				fileStream.Flush();
			}
			if (fileStream != null)
			{
				fileStream.Dispose();
				fileStream = null;
			}
			if (!success)
			{
				IOUtils.RemoveFile(LocalFilePath);
			}
			if (reader != null)
			{
				reader.OnClose();
			}
			if (fireEvent)
			{
				if (success && this.OnTransferFinished != null)
				{
					this.OnTransferFinished(this);
				}
				else if (!success && this.OnTransferFail != null)
				{
					this.OnTransferFail(this);
				}
			}
		}
	}
}