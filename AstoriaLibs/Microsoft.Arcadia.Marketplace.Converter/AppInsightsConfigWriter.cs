using System;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter
{
	public sealed class AppInsightsConfigWriter
	{
		private string outputFilePath;

		public string InstrumentationKey { get; set; }

		public AppInsightsConfigWriter(string outputFilePath)
		{
			if (string.IsNullOrEmpty(outputFilePath))
			{
				throw new ArgumentException("File path is null or empty", "outputFilePath");
			}
			this.outputFilePath = outputFilePath;
		}

		public void WriteToFile()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<ApplicationInsights xmlns=\"http://schemas.microsoft.com/ApplicationInsights/2013/Settings\" schemaVersion=\"2014-05-30\">" + Environment.NewLine + "   <InstrumentationKey>key</InstrumentationKey>" + Environment.NewLine + "</ApplicationInsights>" + Environment.NewLine;
			if (string.IsNullOrEmpty(InstrumentationKey))
			{
				throw new ConverterException("Instrumentation key is required");
			}
			XmlDocWriter xmlDocWriter = new XmlDocWriter(input, InputType.XmlString);
			xmlDocWriter.AddDefaultNamespace("dft", "http://schemas.microsoft.com/ApplicationInsights/2013/Settings");
			xmlDocWriter.SetElementInnerText(XmlUtilites.MakeElementPath("dft", "ApplicationInsights", "InstrumentationKey"), InstrumentationKey);
			xmlDocWriter.WriteToFile(outputFilePath);
		}
	}
}