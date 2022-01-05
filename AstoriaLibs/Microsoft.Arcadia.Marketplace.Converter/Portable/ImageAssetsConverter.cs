using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Parsers;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
	public class ImageAssetsConverter : IDisposable
	{
		internal class IconResource : IEquatable<IconResource>, IComparable<IconResource>
		{
			internal int Delta { get; private set; }

			internal string ResourcePath { get; private set; }

			internal IconResource(int delta, string resourcePath)
			{
				Delta = delta;
				ResourcePath = resourcePath;
			}

			public int CompareTo(IconResource other)
			{
				if (other == null)
				{
					return 1;
				}
				return Delta.CompareTo(other.Delta);
			}

			public override bool Equals(object other)
			{
				IconResource iconResource = other as IconResource;
				if (other == null || iconResource == null)
				{
					return false;
				}
				return Equals(iconResource);
			}

			public bool Equals(IconResource other)
			{
				if (other == null)
				{
					return false;
				}
				return Delta.Equals(other.Delta);
			}

			public override int GetHashCode()
			{
				return Delta + ResourcePath.GetHashCode();
			}
		}

		private PortableZipReader apkZipReader;

		private bool hasDisposed;

		private CachedImageLoader cachedImageLoader;

		private object lockObject = new object();

		private SortedDictionary<string, Color> calculatedBackgroundColors;

		private bool usingDefaultIcon;

		public Color? CalculatedBackgroundColor
		{
			get
			{
				lock (lockObject)
				{
					if (calculatedBackgroundColors.Count() > 0)
					{
						LoggerCore.Log("Calculated background color: {0}", calculatedBackgroundColors.First().Value);
						return calculatedBackgroundColors.First().Value;
					}
				}
				return null;
			}
		}

		public PackageObjectDefaults PackageDefaults { get; private set; }

		private ApkObjectModel ApkModel { get; set; }

		private AppxPackageType AppxType { get; set; }

		private AssetsWriter AssetsWriter { get; set; }

		private ManifestWriter ManifestWriter { get; set; }

		private IPortableRepositoryHandler Repository { get; set; }

		public ImageAssetsConverter(ApkObjectModel apkObjectModel, AppxPackageType appxType, ManifestWriter manifestWriter, AssetsWriter assetsWriter, IPortableRepositoryHandler repository)
			: this(apkObjectModel, appxType, manifestWriter, assetsWriter, repository, null)
		{
		}

		public ImageAssetsConverter(ApkObjectModel apkObjectModel, AppxPackageType appxType, ManifestWriter manifestWriter, AssetsWriter assetsWriter, IPortableRepositoryHandler repository, PackageObjectDefaults objectDefaults)
		{
			ApkModel = apkObjectModel;
			AppxType = appxType;
			ManifestWriter = manifestWriter;
			AssetsWriter = assetsWriter;
			Repository = repository;
			calculatedBackgroundColors = new SortedDictionary<string, Color>();
			apkZipReader = PortableZipReader.Open(Repository.RetrievePackageFilePath());
			cachedImageLoader = new CachedImageLoader();
			PackageDefaults = objectDefaults;
			if (ApkModel != null && ApkModel.ManifestInfo != null && ApkModel.ManifestInfo.Application != null && ApkModel.ManifestInfo.Application.Icon == null && PackageDefaults != null)
			{
				usingDefaultIcon = true;
			}
		}

		public string GenerateOnePreview(AppxImageType imageType)
		{
			ImageConfig highestDpiImageConfig = ImageConfigGenerator.GetHighestDpiImageConfig(AppxType, imageType);
			string empty = string.Empty;
			string result = null;
			ApkResource apkResourceForAppxImageTypeAndConfig = GetApkResourceForAppxImageTypeAndConfig(imageType, highestDpiImageConfig);
			if (apkResourceForAppxImageTypeAndConfig != null)
			{
				GenerateDefaultAppxImageFromAnApkResource(imageType, highestDpiImageConfig, apkResourceForAppxImageTypeAndConfig, buildPreview: true);
				result = AssetsWriter.GetImageAssetFilePath(empty, imageType, highestDpiImageConfig.ScaleQualifier);
			}
			else if (imageType != AppxImageType.TileLogoWide && imageType != AppxImageType.TileLogoLarge)
			{
				ApkResource resource = ApkResourceHelper.GetResource(GetApplicationIcon(), ApkModel.Resources);
				GenerateDefaultAppxImageFromAnApkResource(imageType, highestDpiImageConfig, resource, buildPreview: true);
				result = AssetsWriter.GetImageAssetFilePath(empty, imageType, highestDpiImageConfig.ScaleQualifier);
			}
			return result;
		}

		public void WriteImageAssets()
		{
			LoadAndCacheApkImages();
			Parallel.Invoke(WriteLogos, WriteSplashScreen);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected ApkResource GetApkResourceForAppxImageTypeAndConfig(AppxImageType imageType, ImageConfig config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			ApkResource result = null;
			IReadOnlyCollection<ManifestApplicationMetadata> metadataElements = ApkModel.ManifestInfo.Application.MetadataElements;
			if (metadataElements == null)
			{
				LoggerCore.Log("No metadata found APK manifest.");
				return null;
			}
			foreach (ManifestApplicationMetadata item in metadataElements)
			{
				if (item.IsValidAppxResource && item.PackageType == AppxType && item.ImageType == imageType && item.ScaleQualifier.Equals(config.ScaleQualifier))
				{
					return ApkResourceHelper.GetResource(item.Resource, ApkModel.Resources);
				}
			}
			return result;
		}

		protected string SelectSourceImage(string sourceApkFilesCacheFolder, IReadOnlyCollection<ApkResourceValue> resValues, ImageConfig configure)
		{
			if (resValues == null)
			{
				throw new ArgumentNullException("resValues");
			}
			if (configure == null)
			{
				throw new ArgumentNullException("configure");
			}
			List<IconResource> list = new List<IconResource>();
			List<IconResource> list2 = new List<IconResource>();
			foreach (ApkResourceValue resValue in resValues)
			{
				string relativeDrawableFilePath = ApkResourceHelper.GetRelativeDrawableFilePath(resValue, ApkModel.Resources);
				if (string.IsNullOrWhiteSpace(relativeDrawableFilePath))
				{
					continue;
				}
				string text = null;
				text = ((!usingDefaultIcon) ? apkZipReader.ExtractFileFromZip(relativeDrawableFilePath, sourceApkFilesCacheFolder) : PackageDefaults.ApplicationIconFilePath);
				if (text != null)
				{
					Image result = cachedImageLoader.LoadImageAsync(text).Result;
					int num = result.Height - configure.HeightPixel + result.Width - configure.WidthPixel;
					if (num >= 0)
					{
						list.Add(new IconResource(num, text));
					}
					else
					{
						list2.Add(new IconResource(Math.Abs(num), text));
					}
				}
			}
			list.Sort();
			list2.Sort();
			if (list.Count > 0)
			{
				return list[0].ResourcePath;
			}
			if (list2.Count > 0)
			{
				return list2[0].ResourcePath;
			}
			throw new ConverterException("The APK resource value list is empty, which is not expected!");
		}

		private static IDictionary<string, List<ApkResourceValue>> GroupApkResourceByLocale(ApkResource oneApkResource)
		{
			IDictionary<string, List<ApkResourceValue>> dictionary = new Dictionary<string, List<ApkResourceValue>>();
			foreach (ApkResourceValue value2 in oneApkResource.Values)
			{
				if (value2.ResourceType != ApkResourceType.Drawable)
				{
					throw new ConverterException("The expected resource type is DRAWABLE");
				}
				string key = string.Empty;
				if (value2.Config.Locale != null)
				{
					key = value2.Config.Locale;
				}
				List<ApkResourceValue> value = null;
				if (!dictionary.TryGetValue(key, out value))
				{
					value = (dictionary[key] = new List<ApkResourceValue>());
				}
				value.Add(value2);
			}
			return dictionary;
		}

		private void LoadAndCacheApkImages()
		{
			ApkResource resource = ApkResourceHelper.GetResource(GetApplicationIcon(), ApkModel.Resources);
			Parallel.ForEach(resource.Values, delegate (ApkResourceValue oneResValue)
			{
				string relativeDrawableFilePath = ApkResourceHelper.GetRelativeDrawableFilePath(oneResValue, ApkModel.Resources);
				if (!string.IsNullOrWhiteSpace(relativeDrawableFilePath))
				{
					string text = null;
					text = ((!usingDefaultIcon) ? PortableZipUtils.ExtractFileFromZip(Repository.RetrievePackageFilePath(), relativeDrawableFilePath, Repository.RetrievePackageExtractionPath()) : PackageDefaults.ApplicationIconFilePath);
					if (text != null)
					{
						cachedImageLoader.LoadImageAsync(text).Wait();
					}
				}
			});
		}

		private string GenerateAppxImageFromImageBitmap(Image imageBitmap, string locale, AppxImageType imageType, ImageConfig config, bool buildPreview)
		{
			Color result = AssetsWriter.ConvertApkImageToAppxImage(locale, imageBitmap, config, imageType, buildPreview, CalculatedBackgroundColor).Result;
			lock (lockObject)
			{
				if (!calculatedBackgroundColors.ContainsKey(result.ToString()))
				{
					LoggerCore.Log("Adding background color to calculated background color list", result.ToString());
					calculatedBackgroundColors.Add(result.ToString(), result);
				}
			}
			return AssetsWriter.GetRelativeImagePath(imageType.ToString());
		}

		private string GenerateAppxImageFromImageFilePath(string chosenFilePath, string locale, AppxImageType imageType, ImageConfig config, bool buildPreview)
		{
			string text = null;
			Image result = cachedImageLoader.LoadImageAsync(chosenFilePath).Result;
			Image image = result.Clone();
			if (imageType == AppxImageType.SplashScreen || (image.Height == config.HeightPixel && image.Width == config.WidthPixel))
			{
				return AssetsWriter.WriteVerbatimImage((locale == null) ? string.Empty : locale, image, config, imageType).Result;
			}
			return GenerateAppxImageFromImageBitmap(image, (locale == null) ? string.Empty : locale, imageType, config, buildPreview);
		}

		private string GetSourceFilePath(ApkResource apkResource, string locale, ImageConfig config)
		{
			ApkResourceHelper.ResolveAllStringResourcesAsDrawable(ApkModel, apkResource);
			IDictionary<string, List<ApkResourceValue>> dictionary = GroupApkResourceByLocale(apkResource);
			List<ApkResourceValue> value = null;
			if (!dictionary.TryGetValue(locale, out value))
			{
				using (IEnumerator<KeyValuePair<string, List<ApkResourceValue>>> enumerator = dictionary.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						value = enumerator.Current.Value;
					}
				}
			}
			return SelectSourceImage(Repository.RetrievePackageExtractionPath(), value, config);
		}

		private string GenerateDefaultAppxImageFromAnApkResource(AppxImageType imageType, ImageConfig config, ApkResource apkResource, bool buildPreview)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (apkResource == null)
			{
				throw new ArgumentNullException("apkResource");
			}
			string chosenFilePath = SelectSourceImage(Repository.RetrievePackageExtractionPath(), apkResource.Values, config);
			return GenerateAppxImageFromImageFilePath(chosenFilePath, string.Empty, imageType, config, buildPreview);
		}

		private void GenerateAdditionalAppxImageFromAnApkResource(AppxImageType imageType, ImageConfig config, ApkResource resourceFromApk, bool buildPreview)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (resourceFromApk == null)
			{
				throw new ArgumentNullException("resourceFromApk");
			}
			foreach (ApkResourceValue value in resourceFromApk.Values)
			{
				string relativeDrawableFilePath = ApkResourceHelper.GetRelativeDrawableFilePath(value, ApkModel.Resources);
				if (!string.IsNullOrWhiteSpace(relativeDrawableFilePath) && value.Config != null && !value.Config.Unsupported && !string.IsNullOrEmpty(value.Config.Locale) && LanguageQualifier.IsValidLanguageQualifier(value.Config.Locale))
				{
					string text = apkZipReader.ExtractFileFromZip(relativeDrawableFilePath, Repository.RetrievePackageExtractionPath());
					if (!string.IsNullOrWhiteSpace(text))
					{
						GenerateAppxImageFromImageFilePath(text, value.Config.Locale, imageType, config, buildPreview);
					}
				}
			}
		}

		private string WriteAppxImageForOneImageType(AppxImageType imageType, bool buildPreview)
		{
			string result = null;
			IReadOnlyCollection<ImageConfig> imageConfig = ImageConfigGenerator.GetImageConfig(AppxType, imageType);
			foreach (ImageConfig item in imageConfig)
			{
				ApkResource apkResourceForAppxImageTypeAndConfig = GetApkResourceForAppxImageTypeAndConfig(imageType, item);
				if (apkResourceForAppxImageTypeAndConfig != null)
				{
					result = GenerateDefaultAppxImageFromAnApkResource(imageType, item, apkResourceForAppxImageTypeAndConfig, buildPreview);
					GenerateAdditionalAppxImageFromAnApkResource(imageType, item, apkResourceForAppxImageTypeAndConfig, buildPreview);
				}
				else if (imageType != AppxImageType.TileLogoWide && imageType != AppxImageType.TileLogoLarge)
				{
					ApkResource resource = ApkResourceHelper.GetResource(GetApplicationIcon(), ApkModel.Resources);
					result = GenerateDefaultAppxImageFromAnApkResource(imageType, item, resource, buildPreview);
					GenerateAdditionalAppxImageFromAnApkResource(imageType, item, resource, buildPreview);
				}
			}
			return result;
		}

		private void WriteLogos()
		{
			AppxImageType[] source = ImageConfigGenerator.AllTypeCombinations[AppxType];
			Parallel.ForEach(source, delegate (AppxImageType imageType)
			{
				if (imageType != AppxImageType.SplashScreen)
				{
					string text = WriteAppxImageForOneImageType(imageType, buildPreview: false);
					if (!string.IsNullOrWhiteSpace(text))
					{
						LoggerCore.Log("Image for {0} = {1}", imageType, text);
						switch (imageType)
						{
							case AppxImageType.StoreLogo:
								ManifestWriter.StoreLogo = text;
								break;
							case AppxImageType.AppLogo:
								ManifestWriter.AppLogo = text;
								break;
							case AppxImageType.TileLogoSmall:
								ManifestWriter.TileLogoSmall = text;
								break;
							case AppxImageType.TileLogoMedium:
								ManifestWriter.TileLogoMedium = text;
								break;
							case AppxImageType.TileLogoWide:
								ManifestWriter.TileLogoWide = text;
								break;
							case AppxImageType.TileLogoLarge:
								ManifestWriter.TileLogoLarge = text;
								break;
						}
					}
				}
			});
		}

		private void WriteSplashScreen()
		{
			AppxImageType appxImageType = AppxImageType.SplashScreen;
			string empty = string.Empty;
			IReadOnlyList<ManifestApplicationMetadata> metadataElements = ApkModel.ManifestInfo.Application.MetadataElements;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (ManifestApplicationMetadata item in metadataElements)
			{
				if (item.IsValidAppxResource && item.ImageType == AppxImageType.SplashScreen)
				{
					ApkResource resource = ApkResourceHelper.GetResource(item.Resource, ApkModel.Resources);
					ImageConfig configForScale = ImageConfigGenerator.GetConfigForScale(AppxType, AppxImageType.SplashScreen, item.ScaleQualifier);
					if (configForScale == null)
					{
						throw new ConverterException("Unknown configuration found in meta-data for APPX resource. This should have been caught by Decoder.");
					}
					string sourceFilePath = GetSourceFilePath(resource, empty, configForScale);
					string imageAssetFilePath = AssetsWriter.GetImageAssetFilePath(empty, appxImageType, item.ScaleQualifier);
					PortableUtilsServiceLocator.FileUtils.CopyFile(sourceFilePath, imageAssetFilePath, overwrite: true);
					hashSet.Add(item.ScaleQualifier);
				}
			}
			string relativeImagePath = AssetsWriter.GetRelativeImagePath(appxImageType.ToString());
			if ((AppxType == AppxPackageType.Phone && hashSet.Contains("scale-240")) || (AppxType == AppxPackageType.Tablet && hashSet.Contains("scale-100")))
			{
				LoggerCore.Log("Splash Screen Name: {0}", relativeImagePath);
				ManifestWriter.SplashScreen = relativeImagePath;
				return;
			}
			IReadOnlyCollection<ImageConfig> imageConfig = ImageConfigGenerator.GetImageConfig(AppxType, AppxImageType.SplashScreen);
			foreach (ImageConfig item2 in imageConfig)
			{
				if (!hashSet.Contains(item2.ScaleQualifier))
				{
					ApkResource resource2 = ApkResourceHelper.GetResource(GetApplicationIcon(), ApkModel.Resources);
					string sourceFilePath2 = GetSourceFilePath(resource2, empty, item2);
					_ = AssetsWriter.ProcessAndGenerateSplashScreen(empty, sourceFilePath2, appxImageType, item2, AppxPackageType.Phone, cachedImageLoader).Result;
				}
			}
			LoggerCore.Log("Splash Screen Name: {0}", relativeImagePath);
			ManifestWriter.SplashScreen = relativeImagePath;
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !hasDisposed)
			{
				if (apkZipReader != null)
				{
					apkZipReader.Dispose();
				}
				if (cachedImageLoader != null)
				{
					cachedImageLoader.ClearCache();
				}
				hasDisposed = true;
			}
		}

		private ManifestStringResource GetApplicationIcon()
		{
			ManifestStringResource manifestStringResource = null;
			if (ApkModel.ManifestInfo.Application.Icon == null)
			{
				if (PackageDefaults != null)
				{
					return PackageDefaults.ApplicationIconResource;
				}
				throw new InvalidOperationException("Could not find a suitable icon resource.");
			}
			return ApkModel.ManifestInfo.Application.Icon;
		}
	}
}