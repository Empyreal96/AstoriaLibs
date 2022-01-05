using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
	internal sealed class ResourceItem
	{
		public ResourceKey ResourceKey { get; private set; }

		public ResourceValue SimpleValue { get; private set; }

		public Dictionary<uint, ResourceValue> ComplexValue { get; private set; }

		public ResourceItem()
		{
			ResourceKey = new ResourceKey();
			ComplexValue = new Dictionary<uint, ResourceValue>();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("ResourceItem - \n");
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\t{0}\n", new object[1] { ResourceKey });
			if (SimpleValue != null)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\tSimple Value - {0}\n", new object[1] { SimpleValue });
			}
			if (ComplexValue != null && ComplexValue.Count > 0)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\tComplex Value -\n");
				foreach (KeyValuePair<uint, ResourceValue> item in ComplexValue)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\t\tKey: {0}, Value: {1}\n", new object[2] { item.Key, item.Value });
				}
			}
			return stringBuilder.ToString();
		}

		public void Parse(StreamDecoder streamDecoder)
		{
			ResourceKey.Parse(streamDecoder);
			if (ResourceKey.IsComplexValue())
			{
				for (uint num = 0u; num < ResourceKey.Count; num++)
				{
					uint key = streamDecoder.ReadUint32();
					ResourceValue resourceValue = new ResourceValue();
					resourceValue.Parse(streamDecoder);
					ComplexValue.Add(key, resourceValue);
				}
			}
			else
			{
				SimpleValue = new ResourceValue();
				SimpleValue.Parse(streamDecoder);
			}
		}
	}
}