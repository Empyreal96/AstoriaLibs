using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	public class DecoderValueMustBeStringException : ApkFormatException
	{
		public string AttributeName { get; private set; }

		public string AttributeContent { get; private set; }

		public DecoderValueMustBeStringException()
		{
		}

		public DecoderValueMustBeStringException(string xmlAttributeName, string xmlAttributeValue)
			: base(string.Format(CultureInfo.InvariantCulture, "Node '{0}' cannot contain resource values. Specified value is: '{1}'.", new object[2] { xmlAttributeName, xmlAttributeValue }))
		{
			AttributeName = xmlAttributeName;
			AttributeContent = xmlAttributeValue;
		}

		public DecoderValueMustBeStringException(string message)
			: base(message)
		{
		}

		public DecoderValueMustBeStringException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}