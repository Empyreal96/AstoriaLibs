using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
	internal sealed class XmlDataDecoder
	{
		private readonly string defaultAndroidNamespaceUri = "http://schemas.android.com/apk/res/android";

		private string defaultNamespacePrefix;

		public IReadOnlyList<string> StringPool { get; private set; }

		public IReadOnlyList<uint> ResourceIds { get; private set; }

		public Dictionary<uint, XmlNamespaceMapItem> XmlnsUriToPrefix { get; set; }

		public Dictionary<uint, uint> XmlnsShow { get; set; }

		public uint IndentCount { get; set; }

		public string IndentString => new string(' ', (int)(IndentCount * 2));

		public string DefaultNamespacePrefix
		{
			get
			{
				if (string.IsNullOrWhiteSpace(defaultNamespacePrefix))
				{
					uint prefix = XmlnsUriToPrefix.Single((KeyValuePair<uint, XmlNamespaceMapItem> keyValue) => StringPool[(int)keyValue.Key].Equals(defaultAndroidNamespaceUri)).Value.Prefix;
					defaultNamespacePrefix = StringPool[(int)prefix];
				}
				return defaultNamespacePrefix;
			}
		}

		public XmlDataDecoder(IReadOnlyList<string> stringPool, IReadOnlyList<uint> resourceIds)
		{
			StringPool = stringPool;
			ResourceIds = resourceIds;
			XmlnsUriToPrefix = new Dictionary<uint, XmlNamespaceMapItem>();
			XmlnsShow = new Dictionary<uint, uint>();
			IndentCount = 0u;
		}
	}
}