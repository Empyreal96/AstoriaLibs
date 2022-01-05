using System;

namespace Microsoft.Arcadia.Marketplace.Converter
{
	public class ConverterException : Exception
	{
		public ConverterException()
		{
		}

		public ConverterException(string message)
			: base(message)
		{
		}

		public ConverterException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}