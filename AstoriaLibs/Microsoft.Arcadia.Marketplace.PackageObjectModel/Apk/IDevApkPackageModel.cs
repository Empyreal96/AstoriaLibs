using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public interface IDevApkPackageModel
	{
		IReadOnlyCollection<string> CertificateThumbprints { get; }

		bool HasFile(string relativeFileName);
	}
}