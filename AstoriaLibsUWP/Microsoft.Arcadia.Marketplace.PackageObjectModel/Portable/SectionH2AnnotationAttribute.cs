using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class SectionH2AnnotationAttribute : Attribute
	{
		public ReportSectionH1 ParentSection { get; private set; }

		public int Order { get; private set; }

		public SectionH2AnnotationAttribute(ReportSectionH1 parentSection, int order)
		{
			ParentSection = parentSection;
			Order = order;
		}
	}
}