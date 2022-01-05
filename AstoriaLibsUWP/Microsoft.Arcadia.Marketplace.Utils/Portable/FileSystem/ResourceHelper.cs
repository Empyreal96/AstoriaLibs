using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.FileSystem
{
	public class ResourceHelper : IPortableResourceUtils
	{
		public void WriteNewResX(string filePath, Dictionary<string, string> resValues)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			if (resValues == null)
			{
				throw new ArgumentNullException("resValues");
			}
			Stream stream = null;
			try
			{
				stream = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(filePath);
				using (SimpleResXWriter simpleResXWriter = new SimpleResXWriter(stream))
				{
					stream = null;
					foreach (KeyValuePair<string, string> resValue in resValues)
					{
						LoggerCore.Log("Writing one resources entry, name = {0}, value = {1}", resValue.Key, resValue.Value);
						simpleResXWriter.AddString(resValue.Key, resValue.Value);
					}
				}
			}
			finally
			{
				stream?.Dispose();
			}
		}
	}
}