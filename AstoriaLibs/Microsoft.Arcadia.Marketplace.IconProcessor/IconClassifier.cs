using System;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	public class IconClassifier
	{
		private Image iconImage;

		private BitmapLogger logger;

		private IconSpectrum[] spectrums;

		public bool HasRoundedEdge { get; private set; }

		public bool HasSquareEdge { get; private set; }

		public bool HasUniformColor { get; private set; }

		public Color UniformColor { get; private set; }

		public bool HasGradient { get; private set; }

		public RectangularFrame UniformColorFrame { get; private set; }

		public RectangularFrame GradientColorFrame { get; private set; }

		public Color RecommendedBackgroundColor { get; private set; }

		public int TopMargin { get; private set; }

		public int BottomMargin { get; private set; }

		public int LeftMargin { get; private set; }

		public int RightMargin { get; private set; }

		public bool HasValidResult { get; private set; }

		public IconClassifier(Image icon, BitmapLogger logger)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			iconImage = icon;
			this.logger = logger;
		}

		public void Classify()
		{
			if (iconImage.Height == 0 || iconImage.Width == 0)
			{
				throw new ArgumentException("Zero-size images are not valid");
			}
			HasValidResult = false;
			int num = iconImage.Width * 15 / 100;
			for (int i = 0; i <= num; i++)
			{
				RectangularFrame rectangularFrame = new RectangularFrame();
				rectangularFrame.Margin = i;
				rectangularFrame.CornerRadius = (iconImage.Width + iconImage.Height) * 7 / 200;
				rectangularFrame.Width = iconImage.Width;
				rectangularFrame.Height = iconImage.Height;
				if (TestUniformColorFrame(rectangularFrame))
				{
					break;
				}
			}
			CalculateSpectrums();
			if (!HasSquareEdge && !HasUniformColor)
			{
				RecommendedBackgroundColor = ColorUtils.CalculateRecommendedBackgroundColor(iconImage);
			}
			HasValidResult = true;
		}

		private bool TestUniformColorFrame(RectangularFrame detectionFrame)
		{
			bool hasUniformColor = true;
			bool fitsGradient = true;
			bool foundColor = false;
			Color currentColor = ColorConstants.White;
			detectionFrame.ForEachPixel(delegate (int framePointX, int framePointY)
			{
				Color pixel = iconImage.GetPixel(framePointX, framePointY);
				if (pixel.A < 128)
				{
					fitsGradient = false;
				}
				else
				{
					for (int i = framePointX - 3; i < framePointX + 3; i++)
					{
						if (i >= 0 && i < iconImage.Width && ColorUtils.ColorDistance(pixel, iconImage.GetPixel(i, framePointY)) > 5)
						{
							logger.Log(i, framePointY, ColorConstants.Red);
							fitsGradient = false;
						}
					}
				}
				if (!foundColor)
				{
					currentColor = pixel;
					foundColor = true;
					if (currentColor.A < 128)
					{
						hasUniformColor = false;
					}
				}
				else if (ColorUtils.ColorDistance(currentColor, pixel) > 2)
				{
					hasUniformColor = false;
				}
				if (hasUniformColor)
				{
					logger.Log(framePointX, framePointY, ColorConstants.DarkBlue);
				}
				else if (fitsGradient)
				{
					logger.Log(framePointX, framePointY, ColorConstants.Green);
				}
				else
				{
					logger.Log(framePointX, framePointY, ColorConstants.DarkGray);
				}
				return true;
			});
			if (hasUniformColor && !HasUniformColor)
			{
				UniformColor = currentColor;
				HasUniformColor = true;
				UniformColorFrame = detectionFrame;
			}
			if (fitsGradient && !HasGradient)
			{
				HasUniformColor = false;
				HasGradient = true;
				GradientColorFrame = detectionFrame;
			}
			return hasUniformColor;
		}

		private void CalculateSpectrums()
		{
			spectrums = new IconSpectrum[4];
			spectrums[0] = new IconSpectrum(new Point(0, 0), new Point(0, 1), new Point(1, 0));
			spectrums[1] = new IconSpectrum(new Point(iconImage.Width - 1, 0), new Point(0, 1), new Point(-1, 0));
			spectrums[2] = new IconSpectrum(new Point(0, 0), new Point(1, 0), new Point(0, 1));
			spectrums[3] = new IconSpectrum(new Point(0, iconImage.Height - 1), new Point(1, 0), new Point(0, -1));
			RectangularFrameSide frame = ((UniformColorFrame != null) ? UniformColorFrame.HorizontalSide : null);
			RectangularFrameSide frame2 = ((UniformColorFrame != null) ? UniformColorFrame.VerticalSide : null);
			spectrums[0].Sample(iconImage, frame2, UniformColor);
			spectrums[1].Sample(iconImage, frame2, UniformColor);
			spectrums[2].Sample(iconImage, frame, UniformColor);
			spectrums[3].Sample(iconImage, frame, UniformColor);
			IconSpectrum[] array = spectrums;
			foreach (IconSpectrum iconSpectrum in array)
			{
				iconSpectrum.AnalyzeMargin();
			}
			LeftMargin = spectrums[0].Margin;
			RightMargin = spectrums[1].Margin;
			TopMargin = spectrums[2].Margin;
			BottomMargin = spectrums[3].Margin;
			logger.DrawMarginLine(LeftMargin, 0, 0, 1);
			logger.DrawMarginLine(iconImage.Width - RightMargin - 1, 0, 0, 1);
			logger.DrawMarginLine(0, TopMargin, 1, 0);
			logger.DrawMarginLine(0, iconImage.Height - BottomMargin - 1, 1, 0);
			spectrums[0].AnalyzeEdge(spectrums[2].Margin, spectrums[3].Margin);
			spectrums[1].AnalyzeEdge(spectrums[2].Margin, spectrums[3].Margin);
			spectrums[2].AnalyzeEdge(spectrums[0].Margin, spectrums[1].Margin);
			spectrums[3].AnalyzeEdge(spectrums[0].Margin, spectrums[1].Margin);
			bool flag = true;
			bool flag2 = true;
			bool hasUniformColor = HasUniformColor;
			IconSpectrum[] array2 = spectrums;
			foreach (IconSpectrum iconSpectrum2 in array2)
			{
				if (!iconSpectrum2.HasSquareEdge)
				{
					flag = false;
				}
				if (!iconSpectrum2.HasRoundedEdge)
				{
					flag2 = false;
				}
				if (!iconSpectrum2.HasUniformColor)
				{
					hasUniformColor = false;
				}
			}
			HasRoundedEdge = flag2 && !flag;
			HasSquareEdge = flag;
			HasUniformColor = hasUniformColor;
		}
	}
}