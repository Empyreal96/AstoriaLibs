using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
	[ComImport]
	[Guid("292997C8-FA0D-4FB6-8BC5-366F9382CC81")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAoWInstance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		_AoWInstanceConfig GetConfiguration();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetId([ComAliasName("AowUser.GUID")] out GUID pId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		_AoWUserInstanceState QueryState();

		[MethodImpl(MethodImplOptions.InternalCall)]
		_AoWUserInstanceState SetState([In] uint Flags, [In] _AoWUserInstanceState DesiredState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetAoWBusHandle([ComAliasName("AowUser.ULONG_PTR")] out ulong pHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void MapVfsPath([In] uint Count, [In] ref _AoWInstancePathMapping Paths);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void UnmapVfsPaths([In] uint Count, [In][MarshalAs(UnmanagedType.LPWStr)] ref string pMountPaths);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		IAoWSupportHandler GetSupportHandler();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void MountAppDataFolder([In][MarshalAs(UnmanagedType.LPWStr)] string pPackageRoot, [In][MarshalAs(UnmanagedType.LPWStr)] string pAppPackageFullName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void UnmountAppDataFolder([In][MarshalAs(UnmanagedType.LPWStr)] string pPackageRoot, [In][MarshalAs(UnmanagedType.LPWStr)] string pAppPackageFullName);
	}
}