using System;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class PathSanitizer
	{
		public string Path { get; private set; }

		public PathSanitizer(string folder)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			Path = PortableUtilsServiceLocator.FileUtils.GetFullPath(folder.ToLower());
		}

		public bool IsWithinFolder(string candidatePath)
		{
			if (candidatePath == null)
			{
				throw new ArgumentNullException("candidatePath");
			}
			candidatePath = candidatePath.ToLower();
			return Path.StartsWith(candidatePath, StringComparison.OrdinalIgnoreCase);
		}
	}
}