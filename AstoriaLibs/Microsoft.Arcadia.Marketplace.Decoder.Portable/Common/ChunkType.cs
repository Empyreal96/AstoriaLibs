namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
	public enum ChunkType
	{
		None = 0,
		ResStringPoolType = 1,
		ResTableType = 2,
		ResXmlType = 3,
		ResXmlFirstChunkType = 256,
		ResXmlStartNamespaceType = 256,
		ResXmlEndNamespaceType = 257,
		ResXmlStartElementType = 258,
		ResXmlEndElementType = 259,
		ResXmlCDataType = 260,
		ResXmlLastChunkType = 383,
		ResXmlResourceMapType = 384,
		ResTablePackageType = 512,
		ResTableTypeType = 513,
		ResTableTypeSpecType = 514
	}
}