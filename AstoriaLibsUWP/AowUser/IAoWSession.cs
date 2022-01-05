using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser;

[ComImport]
[Guid("623BBB59-1AA9-46B2-A2E6-E4A749305FCD")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IAoWSession
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	void GetCurrentInstance([MarshalAs(UnmanagedType.Interface)] out IAoWInstance ppInstance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IAoWInstance CreateInstance([In] ref _AoWInstanceConfig pConfig);
}
