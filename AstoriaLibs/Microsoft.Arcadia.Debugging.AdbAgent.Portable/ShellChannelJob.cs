using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal abstract class ShellChannelJob : ChannelJob
	{
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
		private const string ShellPrompt = "\n\nshell@hyperv Shell:/ $ ";

		protected bool IsWithinInteractiveShell { get; set; }

		protected override async Task OnExecuteAsync()
		{
			if (!IsWithinInteractiveShell)
			{
				base.Configuration.AdbServerSender.EnqueueOkay(base.Configuration.LocalId, base.Configuration.RemoteId);
			}
			_ = string.Empty;
			string shellJobResult;
			try
			{
				shellJobResult = await OnExecuteShellCommand();
			}
			catch (Exception exp)
			{
				LoggerCore.Log(exp);
				shellJobResult = "Failure";
			}
			EnqueueDataToAdbServer(shellJobResult);
			if (IsWithinInteractiveShell)
			{
				EnqueueShellPrompt();
				base.Configuration.AdbServerSender.EnqueueOkay(base.Configuration.LocalId, base.Configuration.RemoteId);
			}
			else
			{
				base.Configuration.AdbServerSender.EnqueueClse(base.Configuration.LocalId, base.Configuration.RemoteId);
			}
		}

		protected abstract Task<string> OnExecuteShellCommand();

		private void EnqueueShellPrompt()
		{
			EnqueueDataToAdbServer("\n\nshell@hyperv Shell:/ $ ");
		}

		private void EnqueueDataToAdbServer(string dataToSend)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(dataToSend);
			base.Configuration.AdbServerSender.EnqueueWrte(base.Configuration.LocalId, base.Configuration.RemoteId, bytes, 0, bytes.Length);
		}
	}
}