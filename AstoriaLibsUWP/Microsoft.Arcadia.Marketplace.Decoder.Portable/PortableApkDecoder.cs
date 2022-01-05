using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable
{
	public class PortableApkDecoder
	{
		protected string ManifestAsString { get; set; }

		protected string ApkFilePath { get; set; }

		protected string ApkExtractionPath { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public property is IReadOnly; Needed to support ConcurrentDictionary in inner layers")]
		protected IDictionary<uint, ApkResource> Resources { get; set; }

		protected ApkConfigFile ConfigFile { get; set; }

		protected bool AllowNoResources { get; set; }

		protected async Task DecodeManifestFileAsync()
		{
			if (!string.IsNullOrWhiteSpace(ManifestAsString))
			{
				return;
			}
			string apkManifestFilePath = PortableZipUtils.ExtractFileFromZip(ApkFilePath, "AndroidManifest.xml", ApkExtractionPath);
			if (apkManifestFilePath == null)
			{
				throw new ApkDecoderManifestException("Manifest not found");
			}
			using (XmlDecoder manifestDecoder = new XmlDecoder(apkManifestFilePath))
			{ 
				int num = default(int);
			_ = num;
			_ = 0;
			try
			{
				ManifestAsString = await manifestDecoder.RetrieveStringContentAsync().ConfigureAwait(continueOnCapturedContext: false);
				LoggerCore.Log(ManifestAsString);
			}

			catch (ApkDecoderCommonException)
			{
				throw new ApkDecoderManifestException("ManifestXML");
			}
			}
		}

		protected async Task DecodeResourcesFileAsync()
		{
			if (Resources != null)
			{
				return;
			}
			string apkResourcesFilePath = PortableZipUtils.ExtractFileFromZip(ApkFilePath, "Resources.arsc", ApkExtractionPath);
			if (apkResourcesFilePath != null)
			{
				using (ResourcesDecoder resourcesDecoder = new ResourcesDecoder(apkResourcesFilePath))
				{
					int num = default(int);
					_ = num;
					_ = 0;
					try
					{
						Resources = await resourcesDecoder.RetrieveApkResourcesAsync().ConfigureAwait(continueOnCapturedContext: false);
						LoggerCore.Log(resourcesDecoder.ToString());
						return;
					}
					catch (ApkDecoderCommonException)
					{
						throw new ApkDecoderResourcesException("ResourceFile");
					}
				}
			}
			if (AllowNoResources)
			{
				Resources = new Dictionary<uint, ApkResource>();
				return;
			}
			throw new ApkDecoderResourcesException("Resource file not found");
		}

		protected async Task DecodeConfigFileAsync()
		{
			await Task.Run(delegate
			{
				string text = PortableZipUtils.ExtractFileFromZip(ApkFilePath, "assets\\MicrosoftServices.xml", ApkExtractionPath);
				if (PortableUtilsServiceLocator.FileUtils.FileExists(text))
				{
					try
					{
						XDocument configXml = XDocument.Load(text);
						ApkConfigFile apkConfigFile2 = (ConfigFile = new ApkConfigFile(configXml));
					}
					catch (XmlException exp)
					{
						LoggerCore.Log(exp);
						throw new ApkDecoderConfigException("MicrosoftServices.xml");
					}
				}
			});
		}
	}
}