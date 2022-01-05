using System;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Marketplace.PackageTableObjectModel.Portable
{
	public sealed class PackageTableDataObjectModel
	{
		private const int TimeoutInMilliseconds = 5000;

		private const string BadgingArguments = " d badging ";

		private string packageTableData;

		private string pathToApkFile;

		private string pathToAaptTool;

		private object uniqueLock = new object();

		public string PackageTableDataAsString
		{
			get
			{
				lock (uniqueLock)
				{
					if (packageTableData == null)
					{
						GetPackageTableData();
					}
				}
				return packageTableData;
			}
			private set
			{
				packageTableData = value;
			}
		}

		public PackageTableDataObjectModel(string pathToApkFile, string pathToAaptTool)
		{
			this.pathToApkFile = pathToApkFile;
			this.pathToAaptTool = pathToAaptTool;
		}

		private void GetPackageTableData()
		{
			using (IProcessRunner processRunner = PortableUtilsServiceLocator.ProcessRunnerFactory.Create())
			{
				processRunner.ExePath = pathToAaptTool;
				processRunner.Arguments = " d badging " + pathToApkFile;
				processRunner.StandardErrorEncoding = Encoding.UTF8;
				processRunner.StandardOutputEncoding = Encoding.UTF8;
				if (!processRunner.RunAndWait(5000))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Aapt tool took longer than {0} seconds to complete.", new object[1] { 5 }));
				}
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (processRunner.SupportsStandardOutputRedirection)
				{
					foreach (string item in processRunner.StandardOutput)
					{
						stringBuilder.AppendLine(item);
					}
				}
				if (processRunner.SupportsStandardErrorRedirection)
				{
					foreach (string item2 in processRunner.StandardError)
					{
						stringBuilder2.AppendLine(item2);
					}
				}
				int? exitCode = processRunner.ExitCode;
				if (exitCode.GetValueOrDefault() != 0 || ((!exitCode.HasValue) ? true : false))
				{
					stringBuilder.AppendLine(stringBuilder2.ToString());
				}
				packageTableData = stringBuilder.ToString();
			}
		}
	}
}