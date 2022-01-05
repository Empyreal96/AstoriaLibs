using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("355C3D14-8434-4F35-8377-FDD880F5758A")]
public interface IAoWAppTokenProvider
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: ComAliasName("AowUser.ULONG_PTR")]
	ulong CreateAppImpersonationToken([In][MarshalAs(UnmanagedType.LPWStr)] string pAppPackageFullName, [In] uint cAdditionalCapabilities, [In][MarshalAs(UnmanagedType.LPWStr)] ref string pAdditionalCapabilities, [In][ComAliasName("AowUser.ULONG_PTR")] ulong templateToken);
}
