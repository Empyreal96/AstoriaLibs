using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx")]
	public static class ImageConfigGenerator
	{
		private static IReadOnlyDictionary<AppxPackageType, AppxImageType[]> allTypeCombinationsMap = new Dictionary<AppxPackageType, AppxImageType[]>
	{
		{
			AppxPackageType.Phone,
			new AppxImageType[6]
			{
				AppxImageType.AppLogo,
				AppxImageType.StoreLogo,
				AppxImageType.TileLogoMedium,
				AppxImageType.TileLogoSmall,
				AppxImageType.TileLogoWide,
				AppxImageType.SplashScreen
			}
		},
		{
			AppxPackageType.Tablet,
			new AppxImageType[7]
			{
				AppxImageType.AppLogo,
				AppxImageType.StoreLogo,
				AppxImageType.TileLogoLarge,
				AppxImageType.TileLogoMedium,
				AppxImageType.TileLogoSmall,
				AppxImageType.TileLogoWide,
				AppxImageType.SplashScreen
			}
		}
	};

		public static IReadOnlyDictionary<AppxPackageType, AppxImageType[]> AllTypeCombinations => allTypeCombinationsMap;

		public static IReadOnlyCollection<ImageConfig> GetImageConfig(AppxPackageType typeOfAppx, AppxImageType typeOfImage)
		{
			List<ImageConfig> list = new List<ImageConfig>();
			switch (typeOfAppx)
			{
				case AppxPackageType.Tablet:
					switch (typeOfImage)
					{
						case AppxImageType.AppLogo:
							list.Add(new ImageConfig("scale-80", 24, 24, mandatory: false));
							list.Add(new ImageConfig("scale-100", 30, 30, mandatory: false));
							list.Add(new ImageConfig("scale-140", 42, 42, mandatory: false));
							list.Add(new ImageConfig("scale-180", 54, 54, mandatory: true));
							break;
						case AppxImageType.StoreLogo:
							list.Add(new ImageConfig("scale-100", 50, 50, mandatory: false));
							list.Add(new ImageConfig("scale-140", 70, 70, mandatory: false));
							list.Add(new ImageConfig("scale-180", 90, 90, mandatory: true));
							break;
						case AppxImageType.TileLogoLarge:
							list.Add(new ImageConfig("scale-80", 248, 248, mandatory: false));
							list.Add(new ImageConfig("scale-100", 310, 310, mandatory: false));
							list.Add(new ImageConfig("scale-140", 434, 434, mandatory: false));
							list.Add(new ImageConfig("scale-180", 558, 558, mandatory: true));
							break;
						case AppxImageType.TileLogoMedium:
							list.Add(new ImageConfig("scale-80", 120, 120, mandatory: false));
							list.Add(new ImageConfig("scale-100", 150, 150, mandatory: false));
							list.Add(new ImageConfig("scale-140", 210, 210, mandatory: false));
							list.Add(new ImageConfig("scale-180", 270, 270, mandatory: true));
							break;
						case AppxImageType.TileLogoSmall:
							list.Add(new ImageConfig("scale-80", 56, 56, mandatory: false));
							list.Add(new ImageConfig("scale-100", 70, 70, mandatory: false));
							list.Add(new ImageConfig("scale-140", 98, 98, mandatory: false));
							list.Add(new ImageConfig("scale-180", 126, 126, mandatory: true));
							break;
						case AppxImageType.TileLogoWide:
							list.Add(new ImageConfig("scale-80", 248, 120, mandatory: false));
							list.Add(new ImageConfig("scale-100", 310, 150, mandatory: false));
							list.Add(new ImageConfig("scale-140", 434, 210, mandatory: false));
							list.Add(new ImageConfig("scale-180", 558, 270, mandatory: true));
							break;
						case AppxImageType.SplashScreen:
							list.Add(new ImageConfig("scale-100", 620, 300, mandatory: false));
							list.Add(new ImageConfig("scale-140", 868, 420, mandatory: false));
							list.Add(new ImageConfig("scale-180", 1116, 540, mandatory: true));
							break;
						default:
							throw new PackageObjectModelException("Unknown image type" + typeOfImage);
					}
					break;
				case AppxPackageType.Phone:
					switch (typeOfImage)
					{
						case AppxImageType.AppLogo:
							list.Add(new ImageConfig("scale-100", 44, 44, mandatory: false));
							list.Add(new ImageConfig("scale-140", 62, 62, mandatory: false));
							list.Add(new ImageConfig("scale-240", 106, 106, mandatory: true));
							break;
						case AppxImageType.StoreLogo:
							list.Add(new ImageConfig("scale-100", 50, 50, mandatory: false));
							list.Add(new ImageConfig("scale-140", 70, 70, mandatory: false));
							list.Add(new ImageConfig("scale-240", 120, 120, mandatory: true));
							break;
						case AppxImageType.TileLogoLarge:
							throw new PackageObjectModelException("Phone doesn't support Large tile.");
						case AppxImageType.TileLogoMedium:
							list.Add(new ImageConfig("scale-100", 150, 150, mandatory: false));
							list.Add(new ImageConfig("scale-140", 210, 210, mandatory: false));
							list.Add(new ImageConfig("scale-240", 360, 360, mandatory: true));
							break;
						case AppxImageType.TileLogoSmall:
							list.Add(new ImageConfig("scale-100", 71, 71, mandatory: false));
							list.Add(new ImageConfig("scale-140", 99, 99, mandatory: false));
							list.Add(new ImageConfig("scale-240", 170, 170, mandatory: true));
							break;
						case AppxImageType.TileLogoWide:
							list.Add(new ImageConfig("scale-100", 310, 150, mandatory: false));
							list.Add(new ImageConfig("scale-140", 434, 210, mandatory: false));
							list.Add(new ImageConfig("scale-240", 744, 360, mandatory: true));
							break;
						case AppxImageType.SplashScreen:
							list.Add(new ImageConfig("scale-100", 480, 800, mandatory: true));
							list.Add(new ImageConfig("scale-140", 672, 1120, mandatory: false));
							list.Add(new ImageConfig("scale-240", 1152, 1920, mandatory: true));
							break;
						default:
							throw new PackageObjectModelException("Unknown image type" + typeOfImage);
					}
					break;
				default:
					throw new PackageObjectModelException("Unknown APPX type" + typeOfAppx);
			}
			return list;
		}

		public static ImageConfig GetHighestDpiImageConfig(AppxPackageType typeOfAppx, AppxImageType typeOfImage)
		{
			ImageConfig imageConfig = null;
			foreach (ImageConfig item in GetImageConfig(typeOfAppx, typeOfImage))
			{
				if (imageConfig == null || imageConfig.WidthPixel < item.WidthPixel)
				{
					imageConfig = item;
				}
			}
			return imageConfig;
		}

		public static ImageConfig GetConfigForScale(AppxPackageType typeOfAppx, AppxImageType typeOfImage, string scale)
		{
			if (string.IsNullOrEmpty(scale))
			{
				throw new ArgumentException("scale must not be null or empty.", "scale");
			}
			foreach (ImageConfig item in GetImageConfig(typeOfAppx, typeOfImage))
			{
				if (item.ScaleQualifier.ToUpperInvariant().Equals(scale.ToUpper()))
				{
					return item;
				}
			}
			return null;
		}
	}
}