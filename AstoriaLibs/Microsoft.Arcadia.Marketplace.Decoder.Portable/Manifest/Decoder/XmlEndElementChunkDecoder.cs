using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
	internal sealed class XmlEndElementChunkDecoder
	{
		private readonly XmlEndElementChunk xmlEndElementChunk;

		private readonly XmlDataDecoder xmlDataDecoder;

		internal XmlEndElementChunkDecoder(XmlEndElementChunk xmlEndElementChunk, XmlDataDecoder xmlDataDecoder)
		{
			this.xmlEndElementChunk = xmlEndElementChunk;
			this.xmlDataDecoder = xmlDataDecoder;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}</", new object[1] { xmlDataDecoder.IndentString });
			if (xmlEndElementChunk.Namespace != uint.MaxValue)
			{
				uint prefix = xmlDataDecoder.XmlnsUriToPrefix[xmlEndElementChunk.Namespace].Prefix;
				string text = xmlDataDecoder.StringPool[(int)prefix];
				if (!text.Equals(xmlDataDecoder.DefaultNamespacePrefix))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:", new object[1] { xmlDataDecoder.StringPool[(int)prefix] });
				}
			}
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}>\n", new object[1] { xmlDataDecoder.StringPool[(int)xmlEndElementChunk.Name] });
			return stringBuilder.ToString();
		}
	}
}