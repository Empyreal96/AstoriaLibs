using System;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission
{
	public sealed class AppxCapability
	{
		public string CapabilityName { get; private set; }

		public CapabilityType CapabilityType { get; private set; }

		public AppxCapability(string capabilityName, CapabilityType type)
		{
			if (string.IsNullOrEmpty(capabilityName))
			{
				throw new ArgumentException("Capability name must be provided", "capabilityName");
			}
			CapabilityName = capabilityName;
			CapabilityType = type;
			LoggerCore.Log("APPX Capability - Name: {0}, Type: {1}");
		}
	}
}
