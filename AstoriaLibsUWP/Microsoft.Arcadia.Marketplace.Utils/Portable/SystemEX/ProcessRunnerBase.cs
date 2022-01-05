using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX
{
	public abstract class ProcessRunnerBase : IProcessRunner, IDisposable
	{
		private readonly List<string> standardOutput = new List<string>();

		private readonly List<string> standardError = new List<string>();

		private readonly object syncObject = new object();

		public IReadOnlyList<string> StandardOutput
		{
			get
			{
				lock (syncObject)
				{
					return standardOutput;
				}
			}
		}

		public IReadOnlyList<string> StandardError
		{
			get
			{
				lock (syncObject)
				{
					return standardError;
				}
			}
		}

		public Encoding StandardOutputEncoding { get; set; }

		public Encoding StandardErrorEncoding { get; set; }

		public bool SupportsStandardOutputRedirection { get; protected set; }

		public bool SupportsStandardErrorRedirection { get; protected set; }

		public int? ExitCode { get; protected set; }

		public string ExePath { get; set; }

		public string Arguments { get; set; }

		protected bool HasStarted { get; set; }

		protected bool HasFinished { get; set; }

		public bool RunAndWait(int milliseconds)
		{
			lock (syncObject)
			{
				if (HasStarted)
				{
					throw new InvalidOperationException("Create a new instance of this class to run the process again.");
				}
				HasStarted = true;
			}
			CheckPaths();
			try
			{
				OnLaunchProcess();
				return OnWaitForExitOrTimeout(milliseconds);
			}
			finally
			{
				TerminateProcessIfRunning();
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected abstract void OnLaunchProcess();

		protected abstract bool OnWaitForExitOrTimeout(int timeoutMilliseconds);

		protected abstract void OnTerminateRunningProcess();

		protected virtual void Dispose(bool disposing)
		{
		}

		protected void AddStandardOutputEntry(string entry)
		{
			lock (syncObject)
			{
				standardOutput.Add(entry);
			}
		}

		protected void AddStandardErrorEntry(string entry)
		{
			lock (syncObject)
			{
				standardError.Add(entry);
			}
		}

		private void CheckPaths()
		{
			if (string.Compare(Path.GetFileName(ExePath), ExePath, StringComparison.OrdinalIgnoreCase) != 0)
			{
				IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
				if (!fileUtils.FileExists(ExePath))
				{
					throw new InvalidOperationException("File not found: " + ExePath);
				}
			}
		}

		private void TerminateProcessIfRunning()
		{
			if (HasStarted && !HasFinished)
			{
				OnTerminateRunningProcess();
			}
		}
	}
}