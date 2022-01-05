using System;
using System.IO;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public static class IOUtils
	{
		public static string GetLinuxDirectoryName(string filePath)
		{
			if (string.IsNullOrWhiteSpace("filePath"))
			{
				throw new ArgumentException("Path must not be null or empty.", "filePath");
			}
			return Path.GetDirectoryName(filePath).Replace("\\", "/");
		}

		public static bool RemoveDirectory(string directoryPath)
		{
			if (string.IsNullOrWhiteSpace("filePath"))
			{
				throw new ArgumentException("Path must not be null or empty.", "directoryPath");
			}
			try
			{
				if (PortableUtilsServiceLocator.FileUtils.DirectoryExists(directoryPath))
				{
					PortableUtilsServiceLocator.FileUtils.DeleteDirectory(directoryPath);
					LoggerCore.Log("Deleted {0} successfully.", directoryPath);
					return true;
				}
				return false;
			}
			catch (Exception exp)
			{
				if (!ExceptionUtils.IsIOException(exp))
				{
					throw;
				}
				LoggerCore.Log("Could not delete {0}.", directoryPath);
				return false;
			}
		}

		public static bool RemoveFile(string filePath)
		{
			if (string.IsNullOrWhiteSpace("filePath"))
			{
				throw new ArgumentException("Path must not be null or empty.", "filePath");
			}
			try
			{
				if (PortableUtilsServiceLocator.FileUtils.FileExists(filePath))
				{
					PortableUtilsServiceLocator.FileUtils.DeleteFile(filePath);
					LoggerCore.Log("Deleted {0} successfully.", filePath);
					return true;
				}
				return false;
			}
			catch (Exception exp)
			{
				if (!ExceptionUtils.IsIOException(exp))
				{
					throw;
				}
				LoggerCore.Log("Could not delete {0}.", filePath);
				return false;
			}
		}
	}
}