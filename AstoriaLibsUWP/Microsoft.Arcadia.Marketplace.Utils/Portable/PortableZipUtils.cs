using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public static class PortableZipUtils
	{
		public static string ExtractFileFromZip(string zipFilePath, string entryFileRelativePath, string targetRootFolder)
		{
			using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
			{
				return portableZipReader.ExtractFileFromZip(entryFileRelativePath, targetRootFolder);
			}
			}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "temp")]
		public static void ExtractAllFromZip(string zipFilePath, string targetRootFolder)
		{
			using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
			{
				portableZipReader.ExtractAllFromZip(targetRootFolder);
			}
		}

		public static bool FileWithExtensionExistsInZip(string extension, string zipFilePath)
		{
			using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
			{
				return portableZipReader.FileWithExtensionExistsInZip(extension);
			}
		}

		public static IReadOnlyCollection<string> ExtractFilesWithExtension(string extension, string zipFilePath, string targetRootFolder)
		{
			using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
			{
				return portableZipReader.ExtractFilesWithExtension(extension, targetRootFolder);
			}
		}

		public static bool DirectoryExistsInZip(Regex directory, string zipFilePath)
		{
			using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
			{
				return portableZipReader.DirectoryExistsInZip(directory);
			}
		}
	}
}