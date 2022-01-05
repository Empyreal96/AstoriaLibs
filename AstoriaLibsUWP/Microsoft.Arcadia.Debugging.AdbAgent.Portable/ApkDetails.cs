using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class ApkDetails : IPackageDetails
	{
		public string PackageName { get; private set; }

		public ApkDetails(string apkFileName)
		{
			if (string.IsNullOrWhiteSpace(apkFileName))
			{
				throw new ArgumentException("Must not be null or empty.", "apkFileName");
			}
			PackageName = apkFileName;
		}

		public Task<Stream> RetrievePackageStreamAsync()
		{
			throw new NotImplementedException();
		}
	}
}
