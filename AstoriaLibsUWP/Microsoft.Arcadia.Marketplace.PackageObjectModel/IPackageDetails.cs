using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
	public interface IPackageDetails
	{
		string PackageName { get; }

		Task<Stream> RetrievePackageStreamAsync();
	}
}