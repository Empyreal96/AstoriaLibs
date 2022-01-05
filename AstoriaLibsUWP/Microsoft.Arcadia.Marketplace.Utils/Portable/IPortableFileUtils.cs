using System.IO;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public interface IPortableFileUtils
	{
		long GetFileSize(string filePath);

		string GetFullPath(string filePath);

		bool FileExists(string filePath);

		void DeleteFile(string filePath);

		bool DirectoryExists(string path);

		void CreateDirectory(string path);

		string[] GetDirectories(string path);

		Stream OpenOrCreateFileStream(string filePath);

		Stream OpenReadOnlyFileStream(string filePath);

		void RecursivelyCopyDirectory(string sourceFolderPath, string destinationFolderPath);

		void DeleteDirectory(string targetPath);

		string PathCombine(string path1, string path2);

		void CopyFile(string source, string destination, bool overwrite);
	}
}