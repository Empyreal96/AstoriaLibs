using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
	[ComImport]
	[Guid("C60B7E58-8FE7-4A9C-A86E-42CA6926CC40")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAoWDebugService
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void StartDebugging();

		[MethodImpl(MethodImplOptions.InternalCall)]
		void StopDebugging();
	}
}