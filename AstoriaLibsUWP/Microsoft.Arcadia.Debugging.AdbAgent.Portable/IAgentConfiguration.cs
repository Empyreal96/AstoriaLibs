namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public interface IAgentConfiguration
	{
		string RootAppxTemplateDirectory { get; }

		string AppxLayoutRoot { get; }

		string ToolsDirectory { get; }

		bool EnableInterception { get; }

		bool EnableInteractiveShell { get; }

		string RemoteDataSnifferDirectory { get; }

		string LocalDataSniffedDirectory { get; }
	}
}