using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageTableObjectModel.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ApkObjectModel
	{
		private readonly IDictionary<uint, ApkResource> resources;

		private Dictionary<string, XDocument> decodedXmlFiles;

		private IPortableRepositoryHandler repositoryHandler;

		private PackageTableDataObjectModel packageTable;

		public ManifestInfo ManifestInfo { get; private set; }

		public ApkConfigFile ApkConfigFile { get; private set; }

		public IReadOnlyDictionary<string, XDocument> DecodedXmlFiles => decodedXmlFiles;

		public IDictionary<uint, ApkResource> Resources => resources;

		public IPortableRepositoryHandler RepositoryHandler
		{
			get
			{
				return repositoryHandler;
			}
			set
			{
				repositoryHandler = value;
			}
		}

		public PackageTableDataObjectModel PackageTable
		{
			get
			{
				if (packageTable == null)
				{
					packageTable = new PackageTableDataObjectModel(repositoryHandler.RetrievePackageFilePath(), repositoryHandler.RetrieveAndroidAppPackageToolPath());
				}
				return packageTable;
			}
			private set
			{
				packageTable = value;
			}
		}

		public ApkObjectModel(ManifestInfo manifestInfo, IDictionary<uint, ApkResource> resources, ApkConfigFile configFile)
			: this(manifestInfo, resources, configFile, null)
		{
		}

		public ApkObjectModel(ManifestInfo manifestInfo, IDictionary<uint, ApkResource> resources, ApkConfigFile configFile, IPortableRepositoryHandler repositoryHandler)
		{
			ManifestInfo = manifestInfo;
			this.resources = resources;
			ApkConfigFile = configFile;
			this.repositoryHandler = repositoryHandler;
			decodedXmlFiles = new Dictionary<string, XDocument>();
		}

		public void AddParsedXmlFile(string fileName, XDocument document)
		{
			decodedXmlFiles.Add(fileName, document);
		}

		public string BuildAppxPackageName()
		{
			string text = ManifestInfo.PackageNameResource.Content;
			if (ManifestInfo.PackageNameResource.IsResource)
			{
				ApkResource resource = ApkResourceHelper.GetResource(ManifestInfo.PackageNameResource, resources);
				if (resource.Values.Count <= 0)
				{
					throw new InvalidOperationException("No resource entry for the package name.");
				}
				text = resource.Values[0].Value;
			}
			char[] array = new char[4] { ' ', '_', '-', '.' };
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new InvalidOperationException("Package name is empty or null.");
			}
			char[] array2 = array;
			foreach (char c in array2)
			{
				text = text.Replace(c.ToString(), string.Empty);
			}
			if (text.Length > 35)
			{
				text = text.Substring(0, 35);
			}
			return "Aow" + text;
		}
	}
}