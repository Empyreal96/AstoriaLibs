using System.Runtime.InteropServices;

namespace AowUser;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct __MIDL___MIDL_itf_aowsm_0004_0001_0001
{
	public uint Data1;

	public ushort Data2;

	public ushort Data3;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public byte[] Data4;
}
