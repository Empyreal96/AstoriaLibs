using System;
using System.Globalization;
using System.IO;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.Converter
{
	public sealed class GameServicesConfigWriter
	{
		private GameServicesConfig gameServicesConfig;

		public GameServicesConfigWriter(GameServicesConfig gameServicesConfig)
		{
			if (gameServicesConfig == null)
			{
				throw new ArgumentNullException("gameServicesConfig");
			}
			this.gameServicesConfig = gameServicesConfig;
		}

		public void WriteToFile(string outputFilePath)
		{
			if (string.IsNullOrEmpty(outputFilePath))
			{
				throw new ArgumentException("File path is null or empty", "outputFilePath");
			}
			string format = "{{" + Environment.NewLine + "\"TitleId\" : \"{0}\"," + Environment.NewLine + "\"PrimaryServiceConfigId\" : \"{1}\"," + Environment.NewLine + "\"Sandbox\" : \"{2}\"," + Environment.NewLine + "\"UseDeviceToken\" : \"{3}\"," + Environment.NewLine + "}}";
			string value = string.Format(CultureInfo.InvariantCulture, format, gameServicesConfig.TitleId, gameServicesConfig.PrimaryServiceConfigId, gameServicesConfig.Sandbox, gameServicesConfig.UseDeviceToken);
			using (StreamWriter streamWriter = new StreamWriter(PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(outputFilePath)))
			{
				streamWriter.Write(value);
			}
		}
	}
}