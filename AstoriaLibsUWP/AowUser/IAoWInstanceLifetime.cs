using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser;

[ComImport]
[Guid("782B2D14-179C-4540-BFC8-3599213F764E")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IAoWInstanceLifetime
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	void AcquireExecutionReference([In] uint Flags, [MarshalAs(UnmanagedType.Interface)] out IAoWInstanceExecutionReference ppExecutionReference);
}
