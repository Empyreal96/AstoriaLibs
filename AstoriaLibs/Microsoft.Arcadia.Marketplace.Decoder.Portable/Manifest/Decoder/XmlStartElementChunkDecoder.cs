using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.XmlAttributeValues;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
	internal sealed class XmlStartElementChunkDecoder
	{
		private readonly XmlStartElementChunk xmlStartElementChunk;

		private readonly XmlDataDecoder xmlDataDecoder;

		internal XmlStartElementChunkDecoder(XmlStartElementChunk xmlStartElementChunk, XmlDataDecoder xmlDataDecoder)
		{
			this.xmlStartElementChunk = xmlStartElementChunk;
			this.xmlDataDecoder = xmlDataDecoder;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(xmlDataDecoder.IndentString);
			stringBuilder.Append("<");
			if (xmlStartElementChunk.Namespace != uint.MaxValue)
			{
				uint prefix = xmlDataDecoder.XmlnsUriToPrefix[xmlStartElementChunk.Namespace].Prefix;
				string text = xmlDataDecoder.StringPool[(int)prefix];
				if (!text.Equals(xmlDataDecoder.DefaultNamespacePrefix))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:", new object[1] { xmlDataDecoder.StringPool[(int)prefix] });
				}
			}
			string value = xmlDataDecoder.StringPool[(int)xmlStartElementChunk.Name];
			stringBuilder.Append(value);
			if (xmlDataDecoder.XmlnsShow.Count > 0)
			{
				foreach (KeyValuePair<uint, uint> item in xmlDataDecoder.XmlnsShow)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\n {0} xmlns:", new object[1] { xmlDataDecoder.IndentString });
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}=\"{1}\"", new object[2]
					{
					xmlDataDecoder.StringPool[(int)item.Key],
					xmlDataDecoder.StringPool[(int)item.Value]
					});
				}
				xmlDataDecoder.XmlnsShow.Clear();
			}
			foreach (XmlAttribute attribute in xmlStartElementChunk.Attributes)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\n{0}    ", new object[1] { xmlDataDecoder.IndentString });
				if (attribute.Namespace != uint.MaxValue)
				{
					if (xmlDataDecoder.XmlnsUriToPrefix.ContainsKey(attribute.Namespace))
					{
						uint prefix2 = xmlDataDecoder.XmlnsUriToPrefix[attribute.Namespace].Prefix;
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:", new object[1] { xmlDataDecoder.StringPool[(int)prefix2] });
					}
					else
					{
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:", new object[1] { xmlDataDecoder.DefaultNamespacePrefix });
					}
				}
				string text2 = xmlDataDecoder.StringPool[(int)attribute.Name];
				if (string.IsNullOrWhiteSpace(text2))
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.AppendFormat(CultureInfo.InvariantCulture, "0x{0}", new object[1] { xmlDataDecoder.ResourceIds[(int)attribute.Name].ToString("X8", CultureInfo.InvariantCulture) });
					text2 = XmlResourceIdMap.MapXmlAttributeResourceId(stringBuilder2.ToString());
					if (string.IsNullOrEmpty(text2))
					{
						throw new ApkDecoderManifestException(string.Format(CultureInfo.InvariantCulture, "No Attribute Value found for resource Id {0}", new object[1] { stringBuilder2 }));
					}
				}
				string resourceData = ResourcesHelper.GetResourceData(attribute.Data, xmlDataDecoder.StringPool);
				resourceData = resourceData.Replace("&", "&amp;");
				resourceData = resourceData.Replace("<", "&lt;");
				resourceData = resourceData.Replace("\"", "&quot;");
				resourceData = resourceData.Replace("'", "&apos;");
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}=\"{1}\"", new object[2] { text2, resourceData });
			}
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ">\n");
			return stringBuilder.ToString();
		}
	}
}