using System;

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
	public sealed class ExpMessageArg : IMessageArg
	{
		public object MessageArgument { get; private set; }

		public ExpMessageArg(Exception logExp)
		{
			MessageArgument = logExp;
		}
	}
}