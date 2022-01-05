using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable
{
	public interface IProcessRunner : IDisposable
	{
		int? ExitCode { get; }

		string ExePath { get; set; }

		string Arguments { get; set; }

		bool SupportsStandardOutputRedirection { get; }

		bool SupportsStandardErrorRedirection { get; }

		IReadOnlyList<string> StandardError { get; }

		IReadOnlyList<string> StandardOutput { get; }

		Encoding StandardOutputEncoding { get; set; }

		Encoding StandardErrorEncoding { get; set; }

		bool RunAndWait(int milliseconds);
	}
}