using System;

namespace Microsoft.Arcadia.Debugging.SharedUtils.Portable
{
	public sealed class EtwLogger
	{
		private static IEtwEventStreamProvider instance;

		public static IEtwEventStreamProvider Instance => instance;

		private EtwLogger()
		{
		}

		public static void Initialize(IEtwEventStreamProvider stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			instance = stream;
		}
	}
}
