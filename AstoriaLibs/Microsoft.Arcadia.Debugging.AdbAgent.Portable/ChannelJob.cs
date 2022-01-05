using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal abstract class ChannelJob
	{
		public ChannelJobConfiguration Configuration { get; private set; }

		public async Task ExecuteAsync(ChannelJobConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			Configuration = configuration;
			await OnExecuteAsync();
		}

		protected abstract Task OnExecuteAsync();
	}
}