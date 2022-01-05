using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX
{
	public class PortableProcessRunnerFactory : IProcessRunnerFactory
	{
		public IProcessRunner Create()
		{
			return new PortableProcessRunner();
		}
	}
}