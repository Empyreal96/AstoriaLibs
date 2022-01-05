using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
	public interface IPortableRepositoryHandler
	{
		Task InitializeAsync(IPackageDetails apkDetails);

		string RetrievePackageFilePath();

		string RetrievePackageExtractionPath();

		string RetrieveMakePriToolPath();

		string RetrieveMakePriConfigFilePath();

		string RetrieveAndroidAppPackageToolPath();

		string GetAppxEntryAppTemplatePath(AppxPackageConfiguration config);

		string GetAppxProjectRootFolder(AppxPackageConfiguration config);

		void Clean();
	}
}