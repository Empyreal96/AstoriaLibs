using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class LogAnnotationAttribute : Attribute
	{
		public string MessageCode { get; private set; }

		public ReportSectionH1 Category { get; private set; }

		public WorkerLogProvider Provider { get; private set; }

		public WorkerLogLevel Level { get; private set; }

		public string EnumName { get; internal set; }

		public bool Hidden { get; set; }

		public LogAnnotationAttribute(string messageCode, ReportSectionH1 category)
		{
			MessageCode = messageCode;
			Category = category;
			Hidden = false;
			Provider = WorkerLogProvider.Analyser;
			if (string.IsNullOrWhiteSpace(messageCode))
			{
				throw new ArgumentException("messageCode must not be null or empty.", "messageCode");
			}
			if (messageCode.Length != 8)
			{
				throw new PackageObjectModelException("Incorrect error code length, expecting 8 " + messageCode);
			}
			string text = messageCode.Substring(0, 2).ToUpperInvariant();
			string text2 = messageCode.Substring(2, 2).ToUpperInvariant();
			switch (text)
			{
				case "ER":
					Level = WorkerLogLevel.Error;
					break;
				case "WA":
					Level = WorkerLogLevel.Warning;
					break;
				case "IN":
					Level = WorkerLogLevel.Info;
					break;
				default:
					throw new PackageObjectModelException("Incorrect error code level prefix " + messageCode);
			}
			switch (text2)
			{
				case "AN":
					Provider = WorkerLogProvider.Analyser;
					break;
				case "CO":
					Provider = WorkerLogProvider.Converter;
					break;
				case "DE":
					Provider = WorkerLogProvider.Decoder;
					break;
				case "WA":
					Provider = WorkerLogProvider.WebApi;
					break;
				case "OT":
					Provider = WorkerLogProvider.Other;
					break;
				default:
					throw new PackageObjectModelException("Incorrect error code provider " + messageCode);
			}
		}
	}
}