using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX
{
	[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Type will never be serialized.")]
	[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Type will never be serialized.")]
	public class Win32InteropException : Exception
	{
		public Win32InteropException(int errorCode)
			: base("A Win32 error has occurred. Error Code =" + errorCode.ToString(CultureInfo.InvariantCulture))
		{
			base.HResult = errorCode;
		}

		public Win32InteropException(string errorMessage, int errorCode)
			: base(errorMessage)
		{
		}

		public Win32InteropException(string errorMessage)
			: base(errorMessage)
		{
		}
	}
}