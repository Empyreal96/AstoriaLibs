using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class SectionH1AnnotationAttribute : Attribute
	{
		public int Order { get; private set; }

		public SectionH1AnnotationAttribute(int order)
		{
			Order = order;
		}
	}
}