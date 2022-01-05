using System;
using System.Runtime.InteropServices;

namespace AowUser;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
[ComConversionLoss]
public struct _AoWInstanceConfig
{
	public uint Flags;

	[MarshalAs(UnmanagedType.LPWStr)]
	public string WimPath;

	[MarshalAs(UnmanagedType.LPWStr)]
	public string SupportDll;

	public uint VfsPathCount;

	[ComConversionLoss]
	public IntPtr VfsPaths;
}
