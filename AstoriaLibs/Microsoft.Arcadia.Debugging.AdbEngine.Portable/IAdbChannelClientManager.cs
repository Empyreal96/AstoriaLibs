using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public interface IAdbChannelClientManager
	{
		Task<IAdbChannel> OpenChannelAsync(string name);

		void CloseChannel(IAdbChannel channel);
	}
}
