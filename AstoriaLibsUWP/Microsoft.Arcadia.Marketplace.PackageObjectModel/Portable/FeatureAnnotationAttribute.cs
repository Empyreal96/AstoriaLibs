using System;
using System.Reflection;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class FeatureAnnotationAttribute : Attribute
	{
		public string AggregateFeature { get; set; }

		public uint AggregateFeatureMessageVersion { get; set; }

		public bool VisibleInReport { get; set; }

		public ReportSectionH1 SectionH1 { get; private set; }

		public ReportSectionH2 SectionH2 { get; private set; }

		public WorkerLogProvider Provider { get; private set; }

		public string EnumName { get; internal set; }

		public WorkerLogLevel LogLevel { get; private set; }

		public FeatureAnnotationAttribute(ReportSectionH2 sectionH2, WorkerLogLevel logLevel)
		{
			AggregateFeature = null;
			AggregateFeatureMessageVersion = 0u;
			VisibleInReport = true;
			SectionH2 = sectionH2;
			LogLevel = logLevel;
			Provider = WorkerLogProvider.Analyser;
			FieldInfo runtimeField = typeof(ReportSectionH2).GetRuntimeField(SectionH2.ToString());
			SectionH2AnnotationAttribute customAttribute = runtimeField.GetCustomAttribute<SectionH2AnnotationAttribute>();
			if (customAttribute != null)
			{
				SectionH1 = customAttribute.ParentSection;
			}
		}
	}
}