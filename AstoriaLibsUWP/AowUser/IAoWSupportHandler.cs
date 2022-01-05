using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("38530252-EB81-4164-8854-89E763E44514")]
	public interface IAoWSupportHandler
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void PrepareToStartInstance();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ConnectToInstance();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void StopInstance(int hard);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void Shutdown();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void InstallApk([In][MarshalAs(UnmanagedType.LPWStr)] string pApkAndroidPath, [In][MarshalAs(UnmanagedType.LPWStr)] string pPackageFullName, [In][MarshalAs(UnmanagedType.LPWStr)] string pPackageRoot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void UninstallApk([In][MarshalAs(UnmanagedType.LPWStr)] string pApkAndroidPath, [In][MarshalAs(UnmanagedType.LPWStr)] string pPackageFullName, [In][MarshalAs(UnmanagedType.LPWStr)] string pPackageRoot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void UpdateApk([In][MarshalAs(UnmanagedType.LPWStr)] string pApkAndroidPath, [In][MarshalAs(UnmanagedType.LPWStr)] string pPackageFullName, [In][MarshalAs(UnmanagedType.LPWStr)] string pPackageRoot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void AndroidPackageToWindows([In][MarshalAs(UnmanagedType.LPWStr)] string pAndroidPackageId, [MarshalAs(UnmanagedType.BStr)] out string pPackageFullName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void EnumeratePackages([MarshalAs(UnmanagedType.Interface)] out IAoWPackageEnum ppPackageEnum);
	}
}