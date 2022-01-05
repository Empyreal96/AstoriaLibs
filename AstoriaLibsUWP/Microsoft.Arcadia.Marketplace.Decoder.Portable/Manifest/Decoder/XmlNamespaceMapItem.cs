namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
	internal sealed class XmlNamespaceMapItem
	{
		public uint Prefix { get; private set; }

		public uint Count { get; set; }

		public XmlNamespaceMapItem(uint prefix)
		{
			Prefix = prefix;
			Count = 1u;
		}
	}
}