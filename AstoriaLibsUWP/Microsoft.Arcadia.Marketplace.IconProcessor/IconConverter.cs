using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
	public class IconConverter
	{
		private const int DpiX = 96;

		private const int DpiY = 96;

		private Image iconImage;

		private IconClassifier iconClassifier;

		private int topMargin;

		private int bottomMargin;

		private int leftMargin;

		private int rightMargin;

		public IconConverter(Image image, IconClassifier classifier)
		{
			if (image == null || classifier == null)
			{
				throw new ArgumentException("Arguments are mandatory");
			}
			if (!classifier.HasValidResult)
			{
				throw new ArgumentException("Classifier has not run");
			}
			iconImage = image;
			iconClassifier = classifier;
			topMargin = classifier.TopMargin;
			bottomMargin = classifier.BottomMargin;
			leftMargin = classifier.LeftMargin;
			rightMargin = classifier.RightMargin;
		}

		public async Task<Image> ConvertAsync(int targetWidth, int targetHeight, int targetMargin, bool buildPreview, Color? forceRecommendedColor)
		{
			if (targetWidth > 1024 || targetHeight > 1024)
			{
				throw new ArgumentException("Target size too large for conversion");
			}
			if (iconClassifier.HasSquareEdge)
			{
				return await ResizeImageAsync(targetWidth, targetHeight, 0, null).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (iconClassifier.HasUniformColor && iconClassifier.HasRoundedEdge)
			{
				FillOriginalWithBackground();
				return await ResizeImageAsync(targetWidth, targetHeight, 0, iconClassifier.UniformColor).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (iconClassifier.HasGradient && iconClassifier.HasRoundedEdge)
			{
				FillOriginalWithGradient();
				Image resizedImage = await ResizeImageAsync(targetWidth, targetHeight, 0, null).ConfigureAwait(continueOnCapturedContext: false);
				RectangularFrame.ExtendTheFrame(resizedImage, leftMargin, topMargin);
				return resizedImage;
			}
			Color? backgroundBrush = null;
			if (iconClassifier.HasRoundedEdge && buildPreview)
			{
				backgroundBrush = forceRecommendedColor ?? new Color?(iconClassifier.RecommendedBackgroundColor);
			}
			return await ResizeImageAsync(targetWidth, targetHeight, targetMargin, backgroundBrush);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose")]
		public async Task<Image> GenerateSplashScreenAsync(int targetWidth, int targetHeight, int targetMargin, int backgroundWidth, int backgroundHeight, int verticalOffsetToCenterPixel)
		{
			Image newImage = new Image(backgroundWidth, backgroundHeight, 96.0, 96.0);
			if (iconClassifier.HasUniformColor)
			{
				newImage.FillRectangle(iconClassifier.UniformColor, 0, 0, backgroundWidth, backgroundHeight);
			}
			Image converted = await ConvertAsync(targetWidth, targetHeight, targetMargin, buildPreview: false, null).ConfigureAwait(continueOnCapturedContext: false);
			int offsetX = (backgroundWidth - targetWidth) / 2;
			int offsetY = backgroundHeight / 2 - targetHeight - verticalOffsetToCenterPixel;
			return newImage.Composite(converted, offsetX, offsetY);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose")]
		private async Task<Image> ResizeImageAsync(Image image, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int offsetLeft, int offsetTop, int destWidth, int destHeight, int destImageWidth, int destImageHeight, Color? backgroundBrush = null)
		{
			Image croppedImage = await iconImage.CropAsync(sourceX, sourceY, sourceWidth, sourceHeight).ConfigureAwait(continueOnCapturedContext: false);
			Image resizedImage = await croppedImage.ResizeAsync(destImageWidth, destImageHeight).ConfigureAwait(continueOnCapturedContext: false);
			Image newBaseImage = new Image(destWidth, destHeight, 72.0, 72.0);
			if (backgroundBrush.HasValue)
			{
				newBaseImage.FillRectangle(backgroundBrush.Value, 0, 0, destWidth, destHeight);
			}
			Image compositedImage = newBaseImage.Composite(resizedImage, offsetLeft, offsetTop);
			leftMargin = offsetLeft;
			rightMargin = offsetLeft;
			topMargin = offsetTop;
			bottomMargin = offsetTop;
			return compositedImage;
		}

		private void FillOriginalWithGradient()
		{
			RectangularFrame gradientColorFrame = iconClassifier.GradientColorFrame;
			gradientColorFrame.ExtendTheFrameHorizontally(iconImage);
			leftMargin = (rightMargin = (topMargin = (bottomMargin = gradientColorFrame.Margin)));
		}

		private async Task<Image> ResizeImageAsync(int destWidth, int destHeight, int targetMargin, Color? backgroundBrush)
		{
			int sourceWidth = iconImage.Width - leftMargin - rightMargin;
			int sourceHeight = iconImage.Height - topMargin - bottomMargin;
			if (sourceWidth <= 0 || sourceHeight <= 0)
			{
				return iconImage;
			}
			float widthScaleRatio = (float)(destWidth - targetMargin * 2) * 1f / (float)sourceWidth;
			float heightScaleRatio = (float)(destHeight - targetMargin * 2) * 1f / (float)sourceHeight;
			float scaleRatio = Math.Min(widthScaleRatio, heightScaleRatio);
			int destImageWidth = (int)((float)sourceWidth * scaleRatio);
			int destImageHeight = (int)((float)sourceHeight * scaleRatio);
			return await ResizeImageAsync(offsetLeft: (destWidth - destImageWidth) / 2, offsetTop: (destHeight - destImageHeight) / 2, image: iconImage, sourceX: leftMargin, sourceY: topMargin, sourceWidth: sourceWidth, sourceHeight: sourceHeight, destWidth: destWidth, destHeight: destHeight, destImageWidth: destImageWidth, destImageHeight: destImageHeight, backgroundBrush: backgroundBrush).ConfigureAwait(continueOnCapturedContext: false);
		}

		private void FillOriginalWithBackground()
		{
			RectangularFrame uniformColorFrame = iconClassifier.UniformColorFrame;
			uniformColorFrame.FillMarginsAndCorners(iconImage, iconClassifier.UniformColor);
			leftMargin = (rightMargin = (topMargin = (bottomMargin = 0)));
		}
	}
}