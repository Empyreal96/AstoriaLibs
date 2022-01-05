using System;
using System.Collections.Generic;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx
{
	public static class ScreenOrientationMap
	{
		private static IReadOnlyDictionary<ApkScreenOrientationType, ScreenOrientationItem> androidOrientationToAppxMapping = new Dictionary<ApkScreenOrientationType, ScreenOrientationItem>
	{
		{
			ApkScreenOrientationType.Portrait,
			new ScreenOrientationItem(AppxScreenOrientationCategory.Portrait, new HashSet<AppxScreenOrientationType> { AppxScreenOrientationType.Portrait })
		},
		{
			ApkScreenOrientationType.SensorPortrait,
			new ScreenOrientationItem(AppxScreenOrientationCategory.Portrait, new HashSet<AppxScreenOrientationType>
			{
				AppxScreenOrientationType.Portrait,
				AppxScreenOrientationType.PortraitFlipped
			})
		},
		{
			ApkScreenOrientationType.UserPortrait,
			new ScreenOrientationItem(AppxScreenOrientationCategory.Portrait, new HashSet<AppxScreenOrientationType>
			{
				AppxScreenOrientationType.Portrait,
				AppxScreenOrientationType.PortraitFlipped
			})
		},
		{
			ApkScreenOrientationType.ReversePortrait,
			new ScreenOrientationItem(AppxScreenOrientationCategory.Portrait, new HashSet<AppxScreenOrientationType> { AppxScreenOrientationType.PortraitFlipped })
		},
		{
			ApkScreenOrientationType.Landscape,
			new ScreenOrientationItem(AppxScreenOrientationCategory.Landscape, new HashSet<AppxScreenOrientationType> { AppxScreenOrientationType.Landscape })
		},
		{
			ApkScreenOrientationType.SensorLandscape,
			new ScreenOrientationItem(AppxScreenOrientationCategory.Landscape, new HashSet<AppxScreenOrientationType>
			{
				AppxScreenOrientationType.Landscape,
				AppxScreenOrientationType.LandscapeFlipped
			})
		},
		{
			ApkScreenOrientationType.UserLandscape,
			new ScreenOrientationItem(AppxScreenOrientationCategory.Landscape, new HashSet<AppxScreenOrientationType>
			{
				AppxScreenOrientationType.Landscape,
				AppxScreenOrientationType.LandscapeFlipped
			})
		},
		{
			ApkScreenOrientationType.ReverseLandscape,
			new ScreenOrientationItem(AppxScreenOrientationCategory.Landscape, new HashSet<AppxScreenOrientationType> { AppxScreenOrientationType.LandscapeFlipped })
		}
	};

		private static IReadOnlyDictionary<AppxScreenOrientationType, string> appxScreenOrientionTypeToNameMap = new Dictionary<AppxScreenOrientationType, string>
	{
		{
			AppxScreenOrientationType.Portrait,
			"portrait"
		},
		{
			AppxScreenOrientationType.PortraitFlipped,
			"portraitFlipped"
		},
		{
			AppxScreenOrientationType.Landscape,
			"landscape"
		},
		{
			AppxScreenOrientationType.LandscapeFlipped,
			"landscapeFlipped"
		}
	};

		public static IReadOnlyCollection<IDevReportActivity> GetContradictionRotatingActivites(ApkScreenOrientationType mainActivityOrientation, ICollection<ManifestActivity> supportingActivityList)
		{
			if (supportingActivityList == null)
			{
				throw new ArgumentNullException("supportingActivityList");
			}
			ScreenOrientationItem screenOrientationItem = MapActivityOrientation(mainActivityOrientation);
			List<IDevReportActivity> list = new List<IDevReportActivity>();
			if (screenOrientationItem != null)
			{
				AppxScreenOrientationCategory screenOrientationCategory = screenOrientationItem.ScreenOrientationCategory;
				{
					foreach (ManifestActivity supportingActivity in supportingActivityList)
					{
						ScreenOrientationItem screenOrientationItem2 = MapActivityOrientation(supportingActivity.ScreenOrientation);
						if (screenOrientationItem2 != null && screenOrientationItem2.ScreenOrientationCategory != screenOrientationCategory)
						{
							LoggerCore.Log("Found one activity {0} with contradicting rotational preference {1}.", supportingActivity.NameString, supportingActivity.ScreenOrientation.ToString("F"));
							list.Add(supportingActivity);
						}
					}
					return list;
				}
			}
			LoggerCore.Log("Main activity screen orientation is not mappable, the initial screen orientation will be undefined.");
			return list;
		}

		public static string GetAppxScreenOrientationName(AppxScreenOrientationType screenOrientationType)
		{
			string value = null;
			appxScreenOrientionTypeToNameMap.TryGetValue(screenOrientationType, out value);
			return value;
		}

		public static ScreenOrientationItem MapActivityOrientation(ApkScreenOrientationType activityOrientation)
		{
			ScreenOrientationItem value = null;
			androidOrientationToAppxMapping.TryGetValue(activityOrientation, out value);
			return value;
		}
	}
}