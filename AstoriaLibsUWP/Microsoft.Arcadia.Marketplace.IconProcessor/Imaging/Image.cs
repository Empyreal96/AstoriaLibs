using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Microsoft.Arcadia.Marketplace.IconProcessor.Imaging
{
	public class Image
	{
		private const int BufferPixelSize = 4;

		private const int BufferAlphaChannelOffset = 3;

		private const int BufferBlueChannelOffset = 2;

		private const int BufferRedChannelOffset = 0;

		private const int BufferGreenChannelOffset = 1;

		private const ColorManagementMode ColorManagement = (ColorManagementMode)1;

		private const BitmapAlphaMode AlphaMode = (BitmapAlphaMode)1;

		private const ExifOrientationMode OrientationMode = (ExifOrientationMode)1;

		private const BitmapPixelFormat PixelFormat = (BitmapPixelFormat)30;

		public int Width { get; private set; }

		public int Height { get; private set; }

		public double DpiX { get; private set; }

		public double DpiY { get; private set; }

		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Direct manipulation of underlying buffer is desired.")]
		public byte[] Pixels { get; private set; }

		public Image(int width, int height, double dpiX, double dpiY)
			: this(width, height, dpiX, dpiY, new byte[width * height * 4])
		{
		}

		private Image(int width, int height, double dpiX, double dpiY, byte[] pixelBuffer)
		{
			if (width < 0)
			{
				throw new ArgumentException("width cannot be less than 0.");
			}
			if (height < 0)
			{
				throw new ArgumentException("height cannot be less than 0.");
			}
			if (dpiX < 0.0)
			{
				throw new ArgumentException("dpiX cannot be less than 0.");
			}
			if (dpiY < 0.0)
			{
				throw new ArgumentException("dpiY cannot be less than 0.");
			}
			if (pixelBuffer == null)
			{
				throw new ArgumentNullException("pixelBuffer");
			}
			Pixels = pixelBuffer;
			Width = width;
			Height = height;
			DpiX = dpiX;
			DpiY = dpiY;
		}

		private Image(BitmapDecoder decoder)
		{
			if (decoder == null)
			{
				throw new ArgumentNullException("decoder");
			}
			Pixels = LoadPixelBufferAsync(decoder).Result;
			Width = (int)decoder.OrientedPixelWidth;
			Height = (int)decoder.OrientedPixelHeight;
			DpiX = decoder.DpiX;
			DpiY = decoder.DpiY;
		}

		public static async Task<Image> LoadAsync(string filePath)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			IRandomAccessStream fileStream = await (await StorageFile.GetFileFromPathAsync(filePath)).OpenAsync((FileAccessMode)0);
			try
			{
				BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
				return new Image(decoder);
			}
			finally
			{
				((IDisposable)fileStream)?.Dispose();
			}
		}

		public Image Clone()
		{
			byte[] array = new byte[Width * Height * 4];
			Array.Copy(Pixels, array, Pixels.Length);
			return new Image(Width, Height, DpiX, DpiY, array);
		}

		public Color GetPixel(int x, int y)
		{
			if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1)
			{
				throw new ArgumentException("The X or Y coordinate is invalid.");
			}
			int num = y * Width * 4;
			int num2 = num + x * 4;
			byte b = Pixels[num2 + 2];
			byte g = Pixels[num2 + 1];
			byte r = Pixels[num2];
			byte a = Pixels[num2 + 3];
			return Color.FromArgb(a, r, g, b);
		}

		public void SetPixel(int x, int y, Color color)
		{
			int num = 0;
			if (x > Width - 1)
			{
				throw new ArgumentException("x cannot be greater than the underlying image's width.");
			}
			if (y > Height - 1)
			{
				throw new ArgumentException("y cannot be greater than underlying image's height.");
			}
			int num2 = y * Width * 4;
			num = num2 + x * 4;
			Pixels[num + 3] = color.A;
			Pixels[num + 2] = color.B;
			Pixels[num + 1] = color.G;
			Pixels[num] = color.R;
		}

		public async Task<Image> CropAsync(int cropSourceX, int cropSourceY, int cropSourceWidth, int cropSourceHeight)
		{
			if (cropSourceX < 0)
			{
				throw new ArgumentException("crossSourceX cannot be less than 0.");
			}
			if (cropSourceY < 0)
			{
				throw new ArgumentException("crossSourceY cannot be less than 0.");
			}
			if (cropSourceX + cropSourceWidth > Width)
			{
				throw new ArgumentException("Width of the cropping must not be greater than the sum of the offset and the underlying image's width.", "cropSourceWidth");
			}
			if (cropSourceY + cropSourceHeight > Height)
			{
				throw new ArgumentException("Height of the cropping must not be greater than the sum of the offset and the underlying image's height.", "cropSourceHeight");
			}
			InMemoryRandomAccessStream cropperRas = new InMemoryRandomAccessStream();
			try
			{
				BitmapEncoder cropperEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, (IRandomAccessStream)(object)cropperRas);
				cropperEncoder.SetPixelData((BitmapPixelFormat)30, (BitmapAlphaMode)1, (uint)Width, (uint)Height, DpiX, DpiY, Pixels);
				BitmapBounds cropperBounds = default(BitmapBounds);
				cropperBounds.Height = (uint)cropSourceHeight;
				cropperBounds.Width = (uint)cropSourceWidth;
				cropperBounds.X = (uint)cropSourceX;
				cropperBounds.Y = (uint)cropSourceY;
				cropperEncoder.BitmapTransform.Bounds = (cropperBounds);
				await cropperEncoder.FlushAsync();
				BitmapDecoder postCropDecoder = await BitmapDecoder.CreateAsync((IRandomAccessStream)(object)cropperRas);
				return new Image(postCropDecoder);
			}
			finally
			{
				((IDisposable)cropperRas)?.Dispose();
			}
		}

		public async Task<Image> ResizeAsync(int targetWidth, int targetHeight)
		{
			if (targetWidth < 0)
			{
				throw new ArgumentException("targetWidth must not be less than 0.");
			}
			if (targetHeight < 0)
			{
				throw new ArgumentException("targetHeight must not be less than 0.");
			}
			InMemoryRandomAccessStream scalerRas = new InMemoryRandomAccessStream();
			try
			{
				BitmapEncoder scaleEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, (IRandomAccessStream)(object)scalerRas);
				scaleEncoder.SetPixelData((BitmapPixelFormat)30, (BitmapAlphaMode)1, (uint)Width, (uint)Height, DpiX, DpiY, Pixels);
				scaleEncoder.BitmapTransform.InterpolationMode = ((BitmapInterpolationMode)3);
				scaleEncoder.BitmapTransform.ScaledHeight = ((uint)targetHeight);
				scaleEncoder.BitmapTransform.ScaledWidth = ((uint)targetWidth);
				await scaleEncoder.FlushAsync();
				BitmapDecoder postScaleDecoder = await BitmapDecoder.CreateAsync((IRandomAccessStream)(object)scalerRas);
				return new Image(postScaleDecoder);
			}
			finally
			{
				((IDisposable)scalerRas)?.Dispose();
			}
		}

		public Image Composite(Image foreground, int offsetLeft, int offsetTop)
		{
			if (foreground == null)
			{
				throw new ArgumentNullException("foreground");
			}
			if (foreground.Width + offsetLeft > Width)
			{
				throw new ArgumentException("Superimposed image overflows the background image's width with specified left offset.", "offsetLeft");
			}
			if (foreground.Height + offsetTop > Height)
			{
				throw new ArgumentException("Superimposed image overflows the background image's height with specified top offset.", "offsetTop");
			}
			Image image = Clone();
			for (int i = 0; i < foreground.Width; i++)
			{
				for (int j = 0; j < foreground.Height; j++)
				{
					int x = offsetLeft + i;
					int y = offsetTop + j;
					Color pixel = image.GetPixel(x, y);
					Color pixel2 = foreground.GetPixel(i, j);
					Color color = ColorUtils.BlendColor(pixel, pixel2);
					image.SetPixel(x, y, color);
				}
			}
			return image;
		}

		public void FillRectangle(Color brushColor, int x, int y, int width, int height)
		{
			if (x + width > Width)
			{
				throw new ArgumentException("width cannot be larger than the underlying image width.", "width");
			}
			if (y + height > Height)
			{
				throw new ArgumentException("height cannot be larger than the underlying image height.", "height");
			}
			for (int i = x; i < x + width; i++)
			{
				for (int j = y; j < y + height; j++)
				{
					SetPixel(i, j, brushColor);
				}
			}
		}

		public async Task SaveAsPngAsync(string filePath)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			IRandomAccessStream fileStream = await (await (await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(filePath))).CreateFileAsync(Path.GetFileName(filePath), (CreationCollisionOption)1)).OpenAsync((FileAccessMode)1);
			try
			{
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
				encoder.SetPixelData((BitmapPixelFormat)30, (BitmapAlphaMode)1, (uint)Width, (uint)Height, DpiX, DpiY, Pixels);
				await encoder.FlushAsync();
			}
			finally
			{
				((IDisposable)fileStream)?.Dispose();
			}
		}

		private static async Task<byte[]> LoadPixelBufferAsync(BitmapDecoder decoder)
		{
			PixelDataProvider dataProvider = await decoder.GetPixelDataAsync((BitmapPixelFormat)30, (BitmapAlphaMode)1, new BitmapTransform(), (ExifOrientationMode)1, (ColorManagementMode)1);
			return dataProvider.DetachPixelData();
		}
	}
}