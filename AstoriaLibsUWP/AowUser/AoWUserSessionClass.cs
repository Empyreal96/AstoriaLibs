using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
	[ComImport]
	[ClassInterface(ClassInterfaceType.None)]
	[TypeLibType(TypeLibTypeFlags.FCanCreate)]
	[Guid("910065F3-DB2C-41C8-A50A-AA258AFAC2E8")]
	public class AoWUserSessionClass : IAoWSession, AoWUserSession, IAoWSession2
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AoWUserSessionClass();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void GetCurrentInstance([MarshalAs(UnmanagedType.Interface)] out IAoWInstance ppInstance);

		void IAoWSession.GetCurrentInstance([MarshalAs(UnmanagedType.Interface)] out IAoWInstance ppInstance)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetCurrentInstance
			this.GetCurrentInstance(out ppInstance);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public virtual extern IAoWInstance CreateInstance([In] ref _AoWInstanceConfig pConfig);

		IAoWInstance IAoWSession.CreateInstance([In] ref _AoWInstanceConfig pConfig)
		{
			//ILSpy generated this explicit interface implementation from .override directive in CreateInstance
			return this.CreateInstance(ref pConfig);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void StartInstanceForWindowsApp([In][ComAliasName("AowUser.GUID")] ref GUID instanceIid, out IntPtr ppInstance);

		void IAoWSession2.StartInstanceForWindowsApp([In][ComAliasName("AowUser.GUID")] ref GUID instanceIid, out IntPtr ppInstance)
		{
			//ILSpy generated this explicit interface implementation from .override directive in StartInstanceForWindowsApp
			this.StartInstanceForWindowsApp(ref instanceIid, out ppInstance);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void StartDefaultInstance([In][ComAliasName("AowUser.GUID")] ref GUID instanceIid, out IntPtr ppInstance);

		void IAoWSession2.StartDefaultInstance([In][ComAliasName("AowUser.GUID")] ref GUID instanceIid, out IntPtr ppInstance)
		{
			//ILSpy generated this explicit interface implementation from .override directive in StartDefaultInstance
			this.StartDefaultInstance(ref instanceIid, out ppInstance);
		}
	}
}