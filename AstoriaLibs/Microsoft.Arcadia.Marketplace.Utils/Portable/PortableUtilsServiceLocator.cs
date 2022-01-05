using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public static class PortableUtilsServiceLocator
	{
		private static volatile bool isInit = false;

		private static object initLock = new object();

		public static IPortableFileUtils FileUtils { get; private set; }

		public static IPortableResourceUtils ResourceUtils { get; private set; }

		public static IProcessRunnerFactory ProcessRunnerFactory { get; private set; }

		public static bool Initialized
		{
			get
			{
				return isInit;
			}
			private set
			{
				isInit = value;
			}
		}

		public static void Initialize(IPortableFileUtils fileUtils, IPortableResourceUtils resourceUtils, IProcessRunnerFactory runnerFactory)
		{
			lock (initLock)
			{
				if (Initialized)
				{
					throw new UtilsException("Portable utilities already initialized!");
				}
				FileUtils = fileUtils;
				ResourceUtils = resourceUtils;
				ProcessRunnerFactory = runnerFactory;
				Initialized = true;
			}
		}
	}
}