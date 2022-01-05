using System;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter
{
	public sealed class ConfigWriter
	{
		private string outputFilePath;

		public string AndroidPackageId { get; set; }

		public ConfigWriter(string outputFilePath)
		{
			if (string.IsNullOrEmpty(outputFilePath))
			{
				throw new ArgumentException("File path is null or empty", "outputFilePath");
			}
			this.outputFilePath = outputFilePath;
		}

		public void WriteToFile()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<application>" + Environment.NewLine + "  <packageId>Place Holder</packageId>" + Environment.NewLine + "</application>";
			if (string.IsNullOrEmpty(AndroidPackageId))
			{
				throw new ConverterException("APPX Package Id is required");
			}
			XmlDocWriter xmlDocWriter = new XmlDocWriter(input, InputType.XmlString);
			xmlDocWriter.SetElementInnerText("application/packageId", AndroidPackageId);
			xmlDocWriter.WriteToFile(outputFilePath);
		}
	}
}