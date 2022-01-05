using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.IconProcessor;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "The tool reports spell errors on file extensions")]
	public sealed class AssetsWriter
	{
		private const string AssetsFolderName = "Assets";

		private const string FileExtension = ".png";

		private const int LargeScreenSplashScreenIconSizePixel = 172;

		private const int MediumScreenSplashScreenIconSizePixel = 100;

		private const int SmallScreenSplashScreenIconSizePixel = 72;

		private const int SplashScreenVerticalOffsetToCenterPixel = 13;

		private string rootFolderPath;

		public AssetsWriter(string rootFolderPath)
		{
			if (string.IsNullOrEmpty(rootFolderPath))
			{
				throw new ArgumentException("Folder path is null or empty", "rootFolderPath");
			}
			this.rootFolderPath = rootFolderPath;
		}

		public static string GetRelativeImagePath(string imageName)
		{
			return "Assets\\" + imageName + ".png";
		}

		public string GetImageAssetFilePath(string locale, AppxImageType imageType, string scaleQualifier)
		{
			if (scaleQualifier == null)
			{
				throw new ArgumentNullException("scaleQualifier");
			}
			string text = Path.Combine(new string[2] { rootFolderPath, "Assets" });
			if (!string.IsNullOrEmpty(locale))
			{
				text = Path.Combine(new string[2] { text, locale });
			}
			if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(text))
			{
				PortableUtilsServiceLocator.FileUtils.CreateDirectory(text);
			}
			string text2 = imageType.ToString() + "." + scaleQualifier + ".png";
			return Path.Combine(new string[2] { text, text2 });
		}

		public async Task<string> ProcessAndGenerateSplashScreen(string locale, string sourceFilePath, AppxImageType imageType, ImageConfig config, AppxPackageType packageType, CachedImageLoader imageLoader)
		{
			if (sourceFilePath == null)
			{
				throw new ArgumentNullException("sourceFilePath");
			}
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (locale == null)
			{
				locale = string.Empty;
			}
			if (imageLoader == null)
			{
				throw new ArgumentNullException("cachedImageLoader");
			}
			int widthPixel = config.WidthPixel;
			int heightPixel = config.HeightPixel;
			string scaleQualifier = config.ScaleQualifier;
			int imageSize = 172;
			if (string.Equals(scaleQualifier, "scale-100", StringComparison.Ordinal))
			{
				imageSize = 72;
			}
			else if (string.Equals(scaleQualifier, "scale-140", StringComparison.Ordinal))
			{
				imageSize = 100;
			}
			Image clonedImageBitmap = (await imageLoader.LoadImageAsync(sourceFilePath)).Clone();
			IconClassifier classifier = new IconClassifier(clonedImageBitmap, new BitmapLogger(null));
			classifier.Classify();
			IconConverter converter = new IconConverter(clonedImageBitmap, classifier);
			Image completedImage = await converter.GenerateSplashScreenAsync(imageSize, imageSize, 0, widthPixel, heightPixel, 13);
			string destinationFilePath = GetImageAssetFilePath(locale, imageType, config.ScaleQualifier);
			await completedImage.SaveAsPngAsync(destinationFilePath);
			return destinationFilePath;
		}

		public async Task<Color> ConvertApkImageToAppxImage(string locale, Image imageBitmap, ImageConfig config, AppxImageType imageType, bool buildPreview, Color? forceRecommendedColor)
		{
			if (locale == null)
			{
				locale = string.Empty;
			}
			try
			{
				string destinationFilePath = GetImageAssetFilePath(locale, imageType, config.ScaleQualifier);
				IconClassifier classifier = new IconClassifier(imageBitmap, new BitmapLogger(null));
				classifier.Classify();
				IconConverter converter = new IconConverter(imageBitmap, classifier);
				Image newImage = await converter.ConvertAsync(targetMargin: CalculateTargetMargin(imageType, config.WidthPixel, config.HeightPixel), targetWidth: config.WidthPixel, targetHeight: config.HeightPixel, buildPreview: buildPreview, forceRecommendedColor: forceRecommendedColor);
				await newImage.SaveAsPngAsync(destinationFilePath);
				if (!classifier.HasSquareEdge && !classifier.HasRoundedEdge)
				{
					return Color.Transparent;
				}
				return classifier.RecommendedBackgroundColor;
			}
			catch (Exception)
			{
				LoggerCore.Log("Error when generating image using icon processor.");
				throw;
			}
		}

		public async Task<string> WriteVerbatimImage(string locale, Image imageBitmap, ImageConfig config, AppxImageType imageType)
		{
			string destinationFilePath = GetImageAssetFilePath(locale, imageType, config.ScaleQualifier);
			await imageBitmap.SaveAsPngAsync(destinationFilePath);
			return GetRelativeImagePath(imageType.ToString());
		}

		private static int CalculateTargetMargin(AppxImageType imageType, int widthPixel, int heightPixel)
		{
			if (imageType == AppxImageType.SplashScreen)
			{
				return 0;
			}
			int num = Math.Min(widthPixel, heightPixel);
			if (num <= 4)
			{
				return 0;
			}
			if (num <= 19)
			{
				return 1;
			}
			if (num <= 29)
			{
				return 2;
			}
			if (num <= 48)
			{
				return 3;
			}
			if (num <= 56)
			{
				return 4;
			}
			return num / 14;
		}
	}
}