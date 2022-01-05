using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx
{
	public sealed class ScreenOrientationItem
	{
		public AppxScreenOrientationCategory ScreenOrientationCategory { get; private set; }

		public HashSet<AppxScreenOrientationType> PossibleScreenOrientationTypes { get; private set; }

		public ScreenOrientationItem(AppxScreenOrientationCategory screenOrientationCategory, HashSet<AppxScreenOrientationType> possibleScreenOrientationTypes)
		{
			ScreenOrientationCategory = screenOrientationCategory;
			PossibleScreenOrientationTypes = possibleScreenOrientationTypes;
		}
	}
}