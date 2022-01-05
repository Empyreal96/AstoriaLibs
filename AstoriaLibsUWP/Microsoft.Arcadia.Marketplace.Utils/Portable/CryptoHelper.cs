using System;
using System.Globalization;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public static class CryptoHelper
	{
		public static byte[] ComputeMD5Hash(byte[] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			HashAlgorithmProvider val = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
			IBuffer val2 = CryptographicBuffer.CreateFromByteArray(input);
			IBuffer val3 = val.HashData(val2);
			byte[] result = default(byte[]);
			CryptographicBuffer.CopyToByteArray(val3, out result);
			return result;
		}

		public static string ComputeMD5HashAsHexadecimal(byte[] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			byte[] array = ComputeMD5Hash(input);
			StringBuilder stringBuilder = new StringBuilder(input.Length * 2);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2", CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		public static byte[] ComputeSha1Hash(byte[] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			HashAlgorithmProvider val = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
			IBuffer val2 = CryptographicBuffer.CreateFromByteArray(input);
			IBuffer val3 = val.HashData(val2);
			byte[] result = default(byte[]);
			CryptographicBuffer.CopyToByteArray(val3, out result);
			return result;
		}
	}
}