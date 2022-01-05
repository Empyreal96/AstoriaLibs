using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Utils.Shareable
{
	public class FileUtils : IPortableFileUtils
	{
		public bool FileExists(string filePath)
		{
			return File.Exists(filePath);
		}

		public void DeleteFile(string filePath)
		{
			File.Delete(filePath);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose underlying stream.")]
		public Stream OpenOrCreateFileStream(string filePath)
		{
			return File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose underlying stream.")]
		public Stream OpenReadOnlyFileStream(string filePath)
		{
			return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public string[] GetDirectories(string path)
		{
			return Directory.GetDirectories(path);
		}

		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public string PathCombine(string path1, string path2)
		{
			return Path.Combine(path1, path2);
		}

		public void RecursivelyCopyDirectory(string sourceFolderPath, string destinationFolderPath)
		{
			if (string.IsNullOrWhiteSpace(sourceFolderPath))
			{
				throw new ArgumentException("Folder path is null or empty", "sourceFolderPath");
			}
			if (string.IsNullOrWhiteSpace(destinationFolderPath))
			{
				throw new ArgumentException("Folder path is null or empty", "destinationFolderPath");
			}
			if (string.Compare(sourceFolderPath, destinationFolderPath, StringComparison.OrdinalIgnoreCase) != 0)
			{
				if (Directory.Exists(destinationFolderPath))
				{
					DeleteDirectory(destinationFolderPath);
				}
				Directory.CreateDirectory(destinationFolderPath);
				DirectoryInfo source = new DirectoryInfo(sourceFolderPath);
				DirectoryInfo target = new DirectoryInfo(destinationFolderPath);
				DoRecursiveDirCopy(source, target);
			}
		}

		public void DeleteDirectory(string targetPath)
		{
			Directory.Delete(targetPath, recursive: true);
		}

		public void CopyFile(string source, string destination, bool overwrite)
		{
			File.Copy(source, destination, overwrite);
		}

		public long GetFileSize(string filePath)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			return new FileInfo(filePath).Length;
		}

		public string GetFullPath(string filePath)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			return Path.GetFullPath(filePath);
		}

		private static void DoRecursiveDirCopy(DirectoryInfo source, DirectoryInfo target)
		{
			foreach (FileInfo item in source.EnumerateFiles())
			{
				item.CopyTo(Path.Combine(target.FullName, item.Name), overwrite: true);
			}
			foreach (DirectoryInfo item2 in source.EnumerateDirectories())
			{
				DirectoryInfo target2 = target.CreateSubdirectory(item2.Name);
				DoRecursiveDirCopy(item2, target2);
			}
		}
	}
}