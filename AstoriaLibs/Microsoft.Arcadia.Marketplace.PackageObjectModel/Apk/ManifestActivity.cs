using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	public sealed class ManifestActivity : IDevReportActivity, IDevReportIntentReceiver, IDevReportMetadataContainer
	{
		private List<ManifestIntentFilter> filters;

		private List<DevReportMetadata> metadata;

		private IDictionary<string, ApkScreenOrientationType> screenOrientationMapping = new Dictionary<string, ApkScreenOrientationType>
	{
		{
			"0",
			ApkScreenOrientationType.Landscape
		},
		{
			"1",
			ApkScreenOrientationType.Portrait
		},
		{
			"2",
			ApkScreenOrientationType.User
		},
		{
			"3",
			ApkScreenOrientationType.Behind
		},
		{
			"4",
			ApkScreenOrientationType.Sensor
		},
		{
			"5",
			ApkScreenOrientationType.Nosensor
		},
		{
			"6",
			ApkScreenOrientationType.SensorLandscape
		},
		{
			"7",
			ApkScreenOrientationType.SensorPortrait
		},
		{
			"8",
			ApkScreenOrientationType.ReverseLandscape
		},
		{
			"9",
			ApkScreenOrientationType.ReversePortrait
		},
		{
			"10",
			ApkScreenOrientationType.FullSensor
		},
		{
			"11",
			ApkScreenOrientationType.UserLandscape
		},
		{
			"12",
			ApkScreenOrientationType.UserPortrait
		},
		{
			"13",
			ApkScreenOrientationType.FullUser
		},
		{
			"14",
			ApkScreenOrientationType.Locked
		},
		{
			"landscape",
			ApkScreenOrientationType.Landscape
		},
		{
			"landscapeFlipped",
			ApkScreenOrientationType.LandscapeFlipped
		},
		{
			"portrait",
			ApkScreenOrientationType.Portrait
		},
		{
			"portraitFlipped",
			ApkScreenOrientationType.PortraitFlipped
		},
		{
			"user",
			ApkScreenOrientationType.User
		},
		{
			"behind",
			ApkScreenOrientationType.Behind
		},
		{
			"sensor",
			ApkScreenOrientationType.Sensor
		},
		{
			"nosensor",
			ApkScreenOrientationType.Nosensor
		},
		{
			"sensorLandscape",
			ApkScreenOrientationType.SensorLandscape
		},
		{
			"sensorPortrait",
			ApkScreenOrientationType.SensorPortrait
		},
		{
			"reverseLandscape",
			ApkScreenOrientationType.ReverseLandscape
		},
		{
			"reversePortrait",
			ApkScreenOrientationType.ReversePortrait
		},
		{
			"fullSensor",
			ApkScreenOrientationType.FullSensor
		},
		{
			"userLandscape",
			ApkScreenOrientationType.UserLandscape
		},
		{
			"userPortrait",
			ApkScreenOrientationType.UserPortrait
		},
		{
			"fullUser",
			ApkScreenOrientationType.FullUser
		},
		{
			"locked",
			ApkScreenOrientationType.Locked
		}
	};

		public XElement ActivityXmlElement { get; private set; }

		public ManifestStringResource Name { get; private set; }

		public ManifestStringResource Label { get; private set; }

		public ManifestStringResource Theme { get; private set; }

		public bool HasMainActivity { get; private set; }

		public bool IsLauncherCategory { get; private set; }

		public bool IsHomeCategory { get; private set; }

		public string NameString
		{
			get
			{
				if (Name != null)
				{
					return Name.Content;
				}
				return null;
			}
		}

		public string LaunchModeValue
		{
			get
			{
				if (LaunchModeString != null)
				{
					return LaunchModeString.Content;
				}
				return null;
			}
		}

		public IReadOnlyCollection<IDevReportIntentFilter> IntentFilters => filters;

		public IReadOnlyCollection<DevReportMetadata> Metadata => metadata;

		public ApkScreenOrientationType ScreenOrientation { get; private set; }

		public ManifestString LaunchModeString { get; private set; }

		public IReadOnlyList<ManifestIntentFilter> Filters => filters;

		public ManifestActivity(XElement activityXmlElement)
		{
			if (activityXmlElement == null)
			{
				throw new ArgumentNullException("activityXmlElement");
			}
			ActivityXmlElement = activityXmlElement;
			PopulateFields();
		}

		private void PopulateFields()
		{
			string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(ActivityXmlElement, "http://schemas.android.com/apk/res/android", "name");
			if (!string.IsNullOrEmpty(attributeValueForElement))
			{
				Name = new ManifestStringResource(attributeValueForElement);
			}
			string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(ActivityXmlElement, "http://schemas.android.com/apk/res/android", "label");
			if (!string.IsNullOrEmpty(attributeValueForElement2))
			{
				Label = new ManifestStringResource(attributeValueForElement2);
			}
			string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(ActivityXmlElement, "http://schemas.android.com/apk/res/android", "theme");
			if (!string.IsNullOrEmpty(attributeValueForElement3))
			{
				Theme = new ManifestStringResource(attributeValueForElement3);
			}
			string attributeValueForElement4 = XmlUtilites.GetAttributeValueForElement(ActivityXmlElement, "http://schemas.android.com/apk/res/android", "launchMode");
			if (!string.IsNullOrEmpty(attributeValueForElement4))
			{
				LaunchModeString = new ManifestString("launchMode", attributeValueForElement4);
			}
			filters = ManifestUtilities.GetIntentFilters(ActivityXmlElement);
			string attributeValueForElement5 = XmlUtilites.GetAttributeValueForElement(ActivityXmlElement, "http://schemas.android.com/apk/res/android", "screenOrientation");
			ScreenOrientation = ApkScreenOrientationType.Undeclared;
			if (attributeValueForElement5 != null && screenOrientationMapping.ContainsKey(attributeValueForElement5))
			{
				ScreenOrientation = screenOrientationMapping[attributeValueForElement5];
			}
			foreach (ManifestIntentFilter filter in filters)
			{
				if (filter.Actions.Contains("android.intent.action.MAIN"))
				{
					HasMainActivity = true;
				}
				if (filter.Categories.Contains("android.intent.category.LAUNCHER"))
				{
					IsLauncherCategory = true;
				}
				if (filter.Categories.Contains("android.intent.category.HOME"))
				{
					IsHomeCategory = true;
				}
				if (HasMainActivity && IsLauncherCategory && IsHomeCategory)
				{
					break;
				}
			}
			metadata = ManifestUtilities.GetMetadata(ActivityXmlElement);
		}
	}
}