using System;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter
{
	public sealed class StoreManifestWriter
	{
		private string outputFilePath;

		public StoreManifestWriter(string outputFilePath)
		{
			if (string.IsNullOrEmpty(outputFilePath))
			{
				throw new ArgumentException("File path is null or empty", "outputFilePath");
			}
			this.outputFilePath = outputFilePath;
		}

		public void WriteToFile()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<StoreManifest xmlns=\"http://schemas.microsoft.com/appx/2015/StoreManifest\">" + Environment.NewLine + "<Dependencies>" + Environment.NewLine + "   <MemoryDependency MinForeground=\"300MB\" />" + Environment.NewLine + "</Dependencies>" + Environment.NewLine + "</StoreManifest>";
			XmlDocWriter xmlDocWriter = new XmlDocWriter(input, InputType.XmlString);
			xmlDocWriter.WriteToFile(outputFilePath);
		}
	}
}