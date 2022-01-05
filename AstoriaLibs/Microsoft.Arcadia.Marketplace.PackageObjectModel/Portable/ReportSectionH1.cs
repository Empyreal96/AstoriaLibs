using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public enum ReportSectionH1
	{
		[SectionH1Annotation(1)]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GMS", Justification = "GMS is an acronym for Google Mobile Services.")]
		GMSDependencies,
		[SectionH1Annotation(2)]
		AndroidComponents,
		[SectionH1Annotation(3)]
		Sensors,
		[SectionH1Annotation(4)]
		MediaAndGraphics,
		[SectionH1Annotation(5)]
		ConnectivityAndData,
		InternalAppErrors
	}
}