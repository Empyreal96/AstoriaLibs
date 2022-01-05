using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder
{
	public sealed class ResourcesDecoder : StreamDecoder
	{
		private TableChunk tableChunk;

		private IDictionary<uint, ApkResource> apkResources;

		public ResourcesDecoder(string apkResourcesFilePath)
			: base(apkResourcesFilePath)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("\nResource Groups Count: ");
			if (apkResources != null)
			{
				stringBuilder.Append(apkResources.Count);
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\n Entries: \n");
				foreach (KeyValuePair<uint, ApkResource> apkResource in apkResources)
				{
					stringBuilder.Append(apkResource);
				}
			}
			else
			{
				LoggerCore.Log("Resources have not been decoded yet.");
				stringBuilder.Append("0");
			}
			return stringBuilder.ToString();
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)", Justification = "Internal log message")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Acceptable for tasks")]
		public async Task<IDictionary<uint, ApkResource>> RetrieveApkResourcesAsync()
		{
			return await Task.Run(delegate
			{
				if (apkResources == null)
				{
					LoggerCore.Log("Retrieving apk Resource groups");
					TableChunk tableChunk = RetrieveTableChunk();
					apkResources = ResourcesHelper.GetResourceGroups(tableChunk);
				}
				return apkResources;
			}).ConfigureAwait(continueOnCapturedContext: false);
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)", Justification = "Internal log message")]
		internal TableChunk RetrieveTableChunk()
		{
			if (tableChunk == null)
			{
				LoggerCore.Log("Decoding resources file as table chunk");
				tableChunk = ChunkDecoder.Decode(this) as TableChunk;
			}
			return tableChunk;
		}
	}
}