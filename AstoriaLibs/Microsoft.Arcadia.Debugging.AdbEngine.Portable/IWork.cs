using System.Threading;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public interface IWork
	{
		WaitHandle SignalHandle { get; }

		void DoWork();
	}
}
