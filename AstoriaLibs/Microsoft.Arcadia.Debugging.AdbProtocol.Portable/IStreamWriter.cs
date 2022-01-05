using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public interface IStreamWriter
	{
		Task<int> WriteAsync(byte[] buffer, int startIndex, int bytesToWrite);
	}
}