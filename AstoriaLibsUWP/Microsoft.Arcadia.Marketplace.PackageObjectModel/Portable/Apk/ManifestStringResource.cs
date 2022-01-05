using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
	public sealed class ManifestStringResource
	{
		public const string Sentinel = "@res:";

		public string Content { get; private set; }

		public bool IsResource { get; private set; }

		public uint ResourceId { get; private set; }

		public ManifestStringResource(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			Content = content;
			IsResource = false;
			ResourceId = 0; //originally 0u
			if (content.Length > "@res:".Length)
			{
				string strA = content.Substring(0, "@res:".Length);
				if (string.Compare(strA, "@res:", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					IsResource = true;
					string value = content.Substring("@res:".Length);
					ResourceId = Convert.ToUInt32(value, 16);
				}
			}
		}
	}
}