using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum ScreenDensity : uint
	{
		Default = 0, // 0u
		Low = 120, // 120u
		Medium = 160, // 160u
		TV = 213, // 213u
		High = 240, // 240u
		XHigh = 320, // 320u
		XXHigh = 480, // 480u
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "XXX", Justification = "Following expected naming.")]
		XXXHigh = 640, // 640u
		Any = 65534, // 65534u
		None = 65535 // 65535.u
	}
}