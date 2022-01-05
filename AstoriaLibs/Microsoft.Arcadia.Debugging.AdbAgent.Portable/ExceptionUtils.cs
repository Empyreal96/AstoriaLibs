using System;
using System.IO;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal static class ExceptionUtils
	{
		public static bool IsIOException(Exception exp)
		{
			if (exp == null)
			{
				throw new ArgumentNullException("exp");
			}
			if (!(exp is IOException))
			{
				return exp is UnauthorizedAccessException;
			}
			return true;
		}
	}
}