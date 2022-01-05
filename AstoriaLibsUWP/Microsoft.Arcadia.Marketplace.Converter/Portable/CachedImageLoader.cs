using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
	public class CachedImageLoader
	{
		private ConcurrentDictionary<string, Image> imageCache = new ConcurrentDictionary<string, Image>();

		public async Task<Image> LoadImageAsync(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException("Path must be valid.", "filePath");
			}
			string lowerFilePath = filePath.ToLower();
			Image loadedImage = null;
			if (imageCache.TryGetValue(lowerFilePath, out loadedImage))
			{
				LoggerCore.Log(LoggerCore.LogLevels.Info, "Returning {0} from image cache.", lowerFilePath);
			}
			else
			{
				loadedImage = await Image.LoadAsync(lowerFilePath);
				LoggerCore.Log(LoggerCore.LogLevels.Info, "Adding {0} to image cache.", lowerFilePath);
				imageCache.AddOrUpdate(lowerFilePath, loadedImage, (string key, Image oldValue) => loadedImage);
			}
			return loadedImage;
		}

		public void ClearCache()
		{
			imageCache.Clear();
		}
	}
}