using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder
{
	internal sealed class ResourcesHelper
	{
		[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "temporary")]
		public static IDictionary<uint, ApkResource> GetResourceGroups(TableChunk tableChunk)
		{
			ConcurrentDictionary<uint, ApkResource> resources = new ConcurrentDictionary<uint, ApkResource>();
			IReadOnlyList<string> stringPool = tableChunk.StringPoolChunk.Strings;
			Parallel.ForEach(tableChunk.PackageChunkList, delegate (PackageChunk packageChunk)
			{
				string packageName = packageChunk.PackageName;
				LoggerCore.Log("Package Name: " + packageName);
				uint packageId = packageChunk.PackageId;
				IReadOnlyList<string> typeNameStrings = packageChunk.TypeNameStringsChunk.Strings;
				Parallel.ForEach(packageChunk.TypeRecords, delegate (TypeRecord typeRecord, ParallelLoopState loopState)
				{
					if (typeRecord.TypeChunks.Count > 0)
					{
						uint id = typeRecord.TypeSpecChunk.Id;
						if (id != 0)
						{
							ApkResourceType resourceType = GetResourceType(typeNameStrings[(int)(id - 1)]);
							if (typeRecord.TypeSpecChunk.EntryFlags.Count != typeRecord.TypeSpecChunk.EntryCount)
							{
								throw new ApkDecoderResourcesException("Invalid flag count. Expected: " + typeRecord.TypeSpecChunk.EntryCount);
							}
							for (uint resourceItemId = 0u; resourceItemId < typeRecord.TypeSpecChunk.EntryCount; resourceItemId++)
							{
								ConcurrentBag<ApkResourceValue> apkResourceValues = new ConcurrentBag<ApkResourceValue>();
								Parallel.ForEach(typeRecord.TypeChunks, delegate (TypeChunk typeChunk)
								{
									if (typeChunk.ResourceItems.TryGetValue(resourceItemId, out var value2))
									{
										if (value2 == null)
										{
											throw new ApkDecoderResourcesException("Resource Item can't be null");
										}
										ApkResourceConfig apkResourceConfig = CreateApkResourceConfig(typeRecord, resourceItemId, typeChunk);
										ApkResourceValue apkResourceValue = CreateApkResourceValue(value2, resourceType, apkResourceConfig, stringPool);
										if (apkResourceValue == null)
										{
											throw new ApkDecoderResourcesException("Resource value cannot be null");
										}
										apkResourceValues.Add(apkResourceValue);
									}
								});
								ApkResource value = new ApkResource(apkResourceValues, resourceType);
								uint key = GenerateResourceId(packageId, id, resourceItemId);
								resources.TryAdd(key, value);
							}
						}
					}
				});
			});
			return resources;
		}


		public static string GetResourceData(ResourceValue resourceValue, IReadOnlyList<string> stringPool)
		{
            switch (resourceValue.Type)
            {
				case ResourceValueTypes.Reference:
					return "@res:" + resourceValue.Data.ToString("X", CultureInfo.InvariantCulture);
				case ResourceValueTypes.FirstInt:
					return ((int)resourceValue.Data).ToString(CultureInfo.InvariantCulture);
				case ResourceValueTypes.IntHex:
					return resourceValue.Data.ToString(CultureInfo.InvariantCulture);
				case ResourceValueTypes.IntBoolean:
					return (resourceValue.Data != 0).ToString();
				case ResourceValueTypes.String:
					return stringPool[(int)resourceValue.Data];
				default:
					return string.Concat("{ Type=", resourceValue.Type, ", Data=", resourceValue.Data.ToString("X", CultureInfo.InvariantCulture), "}");

			}
		}

		private static ApkResourceValue CreateApkResourceValue(ResourceItem resourceItem, ApkResourceType resourceType, ApkResourceConfig apkResourceConfig, IReadOnlyList<string> stringPool)
		{
			if (resourceItem.ResourceKey.IsComplexValue())
			{
				return new ApkResourceValue(resourceType, apkResourceConfig, "{Complex Resources}");
			}
			string resourceData = GetResourceData(resourceItem.SimpleValue, stringPool);
			return new ApkResourceValue(resourceType, apkResourceConfig, resourceData);
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)", Justification = "We don't intend to localize log messages")]
		private static bool CheckIfUnSupportedResourcesPresent(uint typeSpecEntryFlag, ResourceConfig config)
		{
			if (((typeSpecEntryFlag & 0x80u) != 0 || (typeSpecEntryFlag & 8u) != 0) && (config.ScreenType & 0xFFFFu) != 0)
			{
				LoggerCore.Log("Resource contains qualifiers related to Orientation or touch-screen.");
				return true;
			}
			if (((typeSpecEntryFlag & (true ? 1u : 0u)) != 0 || (typeSpecEntryFlag & 2u) != 0) && config.Imsi != 0)
			{
				LoggerCore.Log("Resource contains qualifiers related to MNC or MCC");
				return true;
			}
			if (((typeSpecEntryFlag & 0x10u) != 0 || (typeSpecEntryFlag & 0x20u) != 0 || (typeSpecEntryFlag & 0x40u) != 0) && config.Input != 0)
			{
				LoggerCore.Log("Resource contains qualifiers related to Keyboard or navigation.");
				return true;
			}
			if (((typeSpecEntryFlag & 0x1000u) != 0 || (typeSpecEntryFlag & 0x800u) != 0 || (typeSpecEntryFlag & 0x4000u) != 0 || (typeSpecEntryFlag & 0x2000u) != 0) && config.ScreenConfig != 0)
			{
				LoggerCore.Log("Resource contains qualifiers related to Screen configuration.");
				return true;
			}
			return false;
		}

		private static ApkResourceConfig CreateApkResourceConfig(TypeRecord typeRecord, uint resourceItemId, TypeChunk typeChunk)
		{
			ApkResourceConfig apkResourceConfig = new ApkResourceConfig();
			apkResourceConfig.TypeSpecEntry = typeRecord.TypeSpecChunk.EntryFlags[(int)resourceItemId];
			if ((apkResourceConfig.TypeSpecEntry & 4u) != 0)
			{
				apkResourceConfig.Locale = typeChunk.Config.Locale;
			}
			apkResourceConfig.Unsupported = CheckIfUnSupportedResourcesPresent(apkResourceConfig.TypeSpecEntry, typeChunk.Config);
			return apkResourceConfig;
		}

		private static ApkResourceType GetResourceType(string resourceTypeString)
		{
			LoggerCore.Log("Trying to find resource type: {0} in list", resourceTypeString);
			ApkResourceType apkResourceType = ApkResourceType.None;
			switch (resourceTypeString.ToUpperInvariant())
			{
				case "ANIM":
				case "ANIMATION":
				case "ANIMATOR":
					return ApkResourceType.Anim;
				case "COLOR":
					return ApkResourceType.Color;
				case "DRAWABLE":
				case "MIPMAP":
					return ApkResourceType.Drawable;
				case "STRING":
					return ApkResourceType.String;
				case "LAYOUT":
					return ApkResourceType.Layout;
				case "MENU":
					return ApkResourceType.Menu;
				case "STYLE":
					return ApkResourceType.Style;
				case "XML":
					return ApkResourceType.Xml;
				case "ATTR":
					return ApkResourceType.Attr;
				case "RAW":
					return ApkResourceType.Raw;
				case "ID":
					return ApkResourceType.Id;
				case "INTEGER":
					return ApkResourceType.Integer;
				case "DIMEN":
					return ApkResourceType.Dimen;
				case "BOOL":
					return ApkResourceType.Bool;
				case "ARRAY":
					return ApkResourceType.Array;
				case "APKEXPANSIONID":
					return ApkResourceType.ApkExpansionId;
				case "PLURALS":
					return ApkResourceType.Plurals;
				default:
					return ApkResourceType.Unknown;
			}
		}

		private static uint GenerateResourceId(uint packageId, uint typeId, uint itemId)
		{
			return ((packageId & 0xFF) << 24) | ((typeId & 0xFF) << 16) | (itemId & 0xFFFFu);
		}
	}
}