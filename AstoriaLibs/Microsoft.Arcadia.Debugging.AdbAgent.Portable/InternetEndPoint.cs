using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public class InternetEndPoint
	{
		public string Host { get; private set; }

		public int Port { get; private set; }

		public InternetEndPoint(string host, int port)
		{
			if (port <= 0)
			{
				throw new ArgumentException("port must be greater then 0", "port");
			}
			Host = host;
			Port = port;
		}
	}
}