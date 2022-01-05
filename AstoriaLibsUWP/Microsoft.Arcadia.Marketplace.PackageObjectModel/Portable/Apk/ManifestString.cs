using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	public sealed class ManifestString
	{
		public const string Sentinel = "@res:";

		public string Content { get; private set; }

		public ManifestString(string xmlAttributeName, string xmlAttributeValue)
		{
			if (string.IsNullOrWhiteSpace(xmlAttributeName))
			{
				throw new ArgumentException("Value must not be null or whitespace.", "xmlAttributeName");
			}
			if (string.IsNullOrWhiteSpace(xmlAttributeValue))
			{
				throw new ArgumentException("Value must not be null or whitespace.", "xmlAttributeValue");
			}
			if (xmlAttributeValue.StartsWith("@res:", StringComparison.Ordinal))
			{
				throw new DecoderValueMustBeStringException(xmlAttributeName, xmlAttributeValue);
			}
			Content = xmlAttributeValue;
		}
	}
}