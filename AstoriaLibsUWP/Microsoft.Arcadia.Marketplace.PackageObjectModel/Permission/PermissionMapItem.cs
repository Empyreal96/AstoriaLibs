using System.Collections.Generic;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission
{
	public sealed class PermissionMapItem
	{
		public PermissionType PermissionType { get; private set; }

		public IReadOnlyCollection<AppxCapability> MappedCapabilities { get; private set; }

		public PermissionMapItem(PermissionType type, IReadOnlyCollection<AppxCapability> mappedCapabilities)
		{
			PermissionType = type;
			if (type == PermissionType.Present)
			{
				MappedCapabilities = mappedCapabilities;
			}
			LoggerCore.Log("Mapped Capability should not be proivided for Permission of type {0}", type);
		}
	}
}