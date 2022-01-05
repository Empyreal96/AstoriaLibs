using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
	{
public class AdbEngineSocketAcceptException : AdbEngineSocketException
{
	public AdbEngineSocketAcceptException()
	{
	}

	public AdbEngineSocketAcceptException(string message)
		: base(message)
	{
	}

	public AdbEngineSocketAcceptException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
}
