using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
	public interface IStreamReader
	{
		Task<int> ReadAsync(byte[] buffer, int startIndex, int bytesToRead);
	}
}