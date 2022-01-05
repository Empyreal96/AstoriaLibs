using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("75888062-7A77-46C0-9494-E59F2DD4DF0F")]
	public interface IAoWPackageEnum
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		int MoveNext();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetCurrentAndroidPackageName([MarshalAs(UnmanagedType.BStr)] out string pName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetCurrentWindowsPackageFullName([MarshalAs(UnmanagedType.BStr)] out string pName);
	}
}