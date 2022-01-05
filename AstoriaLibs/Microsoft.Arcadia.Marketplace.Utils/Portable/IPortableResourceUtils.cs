using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public interface IPortableResourceUtils
	{
		void WriteNewResX(string filePath, Dictionary<string, string> resValues);
	}
}