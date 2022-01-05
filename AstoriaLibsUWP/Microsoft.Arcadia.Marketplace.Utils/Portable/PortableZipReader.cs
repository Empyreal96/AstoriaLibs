using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public class PortableZipReader : IDisposable
	{
		private static object extractionLock = new object();

		private ZipFile zipFile;

		private bool hasDisposed;

		private PortableZipReader(string zipFilePath)
		{
			if (string.IsNullOrWhiteSpace(zipFilePath))
			{
				throw new ArgumentException("Zip File Path must not be null or empty", "zipFilePath");
			}
			Stream stream = null;
			try
			{
				stream = PortableUtilsServiceLocator.FileUtils.OpenReadOnlyFileStream(zipFilePath);
				zipFile = new ZipFile(stream);
				stream = null;
			}
			finally
			{
				stream?.Dispose();
			}
		}

		public static PortableZipReader Open(string zipFilePath)
		{
			if (string.IsNullOrWhiteSpace(zipFilePath))
			{
				throw new ArgumentException("Zip File Path must not be null or empty", "zipFilePath");
			}
			return new PortableZipReader(zipFilePath);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public string ExtractFileFromZip(string entryFileRelativePath, string targetRootFolder)
		{
			if (string.IsNullOrWhiteSpace(entryFileRelativePath))
			{
				throw new ArgumentException("Entry's Relative File path must not be null or empty", "entryFileRelativePath");
			}
			if (string.IsNullOrWhiteSpace(targetRootFolder))
			{
				throw new ArgumentException("Target root folder must not null or empty.", "targetRootFolder");
			}
			string processedRelativeFilePath = ProcessRelativeFilePathWithSlash(entryFileRelativePath);
			string targetFilePath = Path.Combine(new string[2] { targetRootFolder, processedRelativeFilePath });
			lock (extractionLock)
			{
				if (PortableUtilsServiceLocator.FileUtils.FileExists(targetFilePath))
				{
					LoggerCore.Log("The target file {0} is already extracted. Not extracting twice.", targetFilePath);
					return targetFilePath;
				}
				BufferBlock<Stream> messageQueue = new BufferBlock<Stream>();
				Task task = ConsumeAllStreamsForWritingAsync(targetFilePath, (ISourceBlock<Stream>)(object)messageQueue);
				long foundTargetFile = 0L;
				Parallel.ForEach(zipFile.Cast<ZipEntry>(), delegate (ZipEntry entry, ParallelLoopState lockEntryState)
				{
					string strB = ProcessRelativeFilePathWithSlash(entry.Name);
					if (string.Compare(processedRelativeFilePath, strB, StringComparison.OrdinalIgnoreCase) == 0)
					{
						Interlocked.Increment(ref foundTargetFile);
						string directoryName = Path.GetDirectoryName(targetFilePath);
						if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(directoryName))
						{
							PortableUtilsServiceLocator.FileUtils.CreateDirectory(directoryName);
						}
						DataflowBlock.Post<Stream>((ITargetBlock<Stream>)(object)messageQueue, zipFile.GetInputStream(entry));
						messageQueue.Complete();
						lockEntryState.Stop();
					}
				});
				if (Interlocked.Read(ref foundTargetFile) == 0)
				{
					return null;
				}
				task.Wait();
				return targetFilePath;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "temp")]
		public void ExtractAllFromZip(string targetRootFolder)
		{
			if (string.IsNullOrWhiteSpace(targetRootFolder))
			{
				throw new ArgumentException("Target root folder must not null or empty.", "targetRootFolder");
			}
			foreach (ZipEntry item in zipFile)
			{
				string text = ProcessRelativeFilePathWithSlash(item.Name);
				string text2 = Path.Combine(new string[2] { targetRootFolder, text });
				try
				{
					LoggerCore.Log("Extracting " + text + " to " + text2);
					string directoryName = Path.GetDirectoryName(text2);
					if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(directoryName))
					{
						PortableUtilsServiceLocator.FileUtils.CreateDirectory(directoryName);
					}
					using (Stream stream = zipFile.GetInputStream(item))
					{
						using (Stream destination = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(text2))
						{
							stream.CopyToAsync(destination).Wait();
						}
					}
				}
				catch (Exception exp)
				{
					LoggerCore.Log("Failed to extract path " + text2);
					LoggerCore.Log(exp);
				}
			}
		}

		public bool FileWithExtensionExistsInZip(string extension)
		{
			return zipFile.Cast<ZipEntry>().Any((ZipEntry entry) => entry.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
		}

		public IReadOnlyCollection<string> ExtractFilesWithExtension(string extension, string targetRootFolder)
		{
			if (string.IsNullOrWhiteSpace(extension))
			{
				throw new ArgumentException("Extension must not null or empty.", "extension");
			}
			if (string.IsNullOrWhiteSpace(targetRootFolder))
			{
				throw new ArgumentException("Target root folder must not null or empty.", "targetRootFolder");
			}
			List<string> list = new List<string>();
			IEnumerable<ZipEntry> enumerable = from ZipEntry entry in zipFile
											   where entry.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
											   select entry;
			foreach (ZipEntry item in enumerable)
			{
				list.Add(ExtractFileFromZip(item.Name, targetRootFolder));
			}
			LoggerCore.Log("Extracted {0} file(s) with extension {1} to {2}", list.Count, extension, targetRootFolder);
			return list;
		}

		internal bool DirectoryExistsInZip(Regex directory)
		{
			foreach (ZipEntry item in zipFile)
			{
				Match match = directory.Match(item.Name);
				if (match.Success)
				{
					return true;
				}
			}
			return false;
		}

		private static string ProcessRelativeFilePathWithSlash(string relativeFilePath)
		{
			string text = relativeFilePath.Trim('/', '\\');
			return text.Replace('/', '\\');
		}

		private static async Task ConsumeAllStreamsForWritingAsync(string targetFilePath, ISourceBlock<Stream> sourceStreams)
		{
			while (await DataflowBlock.OutputAvailableAsync<Stream>(sourceStreams).ConfigureAwait(continueOnCapturedContext: false))
			{
				using (Stream stream = DataflowBlock.Receive<Stream>(sourceStreams))
				{
					using (Stream destination = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(targetFilePath))
					{
						stream.CopyToAsync(destination).Wait();
					}
				}
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !hasDisposed)
			{
				zipFile.IsStreamOwner = true;
				zipFile.Close();
				hasDisposed = true;
			}
		}
	}
}