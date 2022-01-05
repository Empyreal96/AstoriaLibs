namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
	internal abstract class XmlElementChunk : XmlItemChunk
	{
		protected internal uint Namespace { get; set; }

		protected internal uint Name { get; set; }

		protected XmlElementChunk()
		{
			Namespace = uint.MaxValue;
			Name = 0; //originally 0u
		}
	}
}