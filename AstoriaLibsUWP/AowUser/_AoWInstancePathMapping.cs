using System.Runtime.InteropServices;

namespace AowUser;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct _AoWInstancePathMapping
{
	[MarshalAs(UnmanagedType.LPWStr)]
	public string WindowsPath;

	[MarshalAs(UnmanagedType.LPWStr)]
	public string MountPath;

	public uint Uid;

	public uint Gid;

	public uint MappingFlags;
}
