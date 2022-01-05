using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class EnumDescriptionAttribute : Attribute
	{
		private readonly string description;

		public string Description => description;

		public EnumDescriptionAttribute(string description)
		{
			this.description = description;
		}
	}
}