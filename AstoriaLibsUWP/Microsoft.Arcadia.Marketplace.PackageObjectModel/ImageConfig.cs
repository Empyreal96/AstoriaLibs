using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
	public sealed class ImageConfig : IEquatable<ImageConfig>
	{
		public string ScaleQualifier { get; private set; }

		public int WidthPixel { get; private set; }

		public int HeightPixel { get; private set; }

		public bool Mandatory { get; private set; }

		public ImageConfig(string scaleQualifier, int widthPixel, int heightPixel, bool mandatory)
		{
			ScaleQualifier = scaleQualifier;
			WidthPixel = widthPixel;
			HeightPixel = heightPixel;
			Mandatory = mandatory;
		}

		public bool Equals(ImageConfig other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			return WidthPixel == other.WidthPixel && HeightPixel == other.HeightPixel && Mandatory.Equals(other.Mandatory) && ScaleQualifier.ToString().Equals(other.ScaleQualifier.ToString());
		}

		public override int GetHashCode()
		{
			int num = 0;
			int num2 = 397;
			num = (num * num2) ^ WidthPixel.GetHashCode();
			num = (num * num2) ^ HeightPixel.GetHashCode();
			num = (num * num2) ^ Mandatory.GetHashCode();
			return (num * num2) ^ ScaleQualifier.GetHashCode();
		}
	}
}