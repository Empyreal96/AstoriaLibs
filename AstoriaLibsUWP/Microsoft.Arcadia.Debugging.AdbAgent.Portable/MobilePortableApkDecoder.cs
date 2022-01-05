using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Decoder.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class MobilePortableApkDecoder : PortableApkDecoder
	{
		private string correlationId;

		public ApkObjectModel ObjModel { get; private set; }

		public MobilePortableApkDecoder(IPortableRepositoryHandler repository, string correlationId)
		{
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			if (correlationId == null)
			{
				throw new ArgumentNullException("correlationId");
			}
			base.ApkFilePath = repository.RetrievePackageFilePath();
			base.ApkExtractionPath = repository.RetrievePackageExtractionPath();
			this.correlationId = correlationId;
			base.AllowNoResources = true;
		}

		public async Task DecodeAsync()
		{
			EtwLogger.Instance.ApkManifestDecoding(correlationId);
			await DecodeManifestFileAsync();
			EtwLogger.Instance.ApkManifestDecoded(correlationId);
			EtwLogger.Instance.ApkResourcesDecoding(correlationId);
			await DecodeResourcesFileAsync();
			EtwLogger.Instance.ApkResourcesDecoded(correlationId);
			await DecodeConfigFileAsync();
			XDocument manifestDocument = XDocument.Parse(base.ManifestAsString);
			ManifestInfo manifestInfo = new ManifestInfo(manifestDocument, (System.Collections.Generic.IDictionary<uint, Marketplace.PackageObjectModel.Portable.Apk.ApkResource>)base.Resources);
			EtwLogger.Instance.ApkManifestInfo(manifestInfo.PackageNameResource.Content, correlationId);
			ObjModel = new ApkObjectModel(manifestInfo, (System.Collections.Generic.IDictionary<uint, Marketplace.PackageObjectModel.Portable.Apk.ApkResource>)base.Resources, base.ConfigFile);
		}
	}
}