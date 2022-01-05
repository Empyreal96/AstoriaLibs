namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx
{
	public class FileTypeAssociation
	{
		public string FileType { get; private set; }

		public string Name { get; private set; }

		public FileTypeAssociation(string name, string fileType)
		{
			FileType = fileType;
			Name = name;
		}
	}
}