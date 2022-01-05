using System;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	public sealed class BitmapLogger
	{
		private Image bitmap;

		public BitmapLogger(Image icon)
		{
			bitmap = icon;
		}

		public void Log(int x, int y, Color color)
		{
			if (bitmap != null)
			{
				bitmap.SetPixel(x, y, color);
			}
		}

		public void Log(Point point, Color color)
		{
			if (point == null)
			{
				throw new ArgumentNullException("point");
			}
			Log(point.X, point.Y, color);
		}

		public void DrawMarginLine(int x, int y, int incrementX, int incrementY)
		{
			if (incrementX == 0 && incrementY == 0)
			{
				throw new ArgumentException("Increment must be specified");
			}
			if (bitmap != null)
			{
				while (x >= 0 && y >= 0 && x < bitmap.Width && y < bitmap.Height)
				{
					bitmap.SetPixel(x, y, ColorConstants.Red);
					x += incrementX;
					y += incrementY;
				}
			}
		}

		public async Task Save(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException("fileName can't be NULL");
			}
			if (bitmap != null)
			{
				await bitmap.SaveAsPngAsync(fileName).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}
}
