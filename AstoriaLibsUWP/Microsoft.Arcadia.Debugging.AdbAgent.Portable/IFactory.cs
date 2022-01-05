using System;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	public interface IFactory : IDisposable
	{
		AppxPackageType AppxPackageType { get; }

		IAgentConfiguration AgentConfiguration { get; }

		IAowInstanceWrapper AowInstance { get; }

		ISocketAcceptWork CreateSocketAcceptWork(InternetEndPoint listenEndPoint);

		ISocketConnectWork CreateSocketConnectWork(InternetEndPoint connectEndPoint, uint attempts);

		ISocketReceiveWork CreateSocketReceiveWork(ISocket socket, string identifier);

		ISocketSendWork CreateSocketSendWork(ISocket socket, string identifier);

		IPackageManager CreatePackageManager();

		IUriLauncher CreateUriLauncher();

		IPortableRepositoryHandler CreateRepository();

		IPortableFileUtils CreatePortableFileUtils();

		IPortableResourceUtils CreatePortableResourceUtils();

		ISystemInformation CreateSystemInformation();

		IProcessRunnerFactory CreateProcessRunnerFactory();

		IShellManager CreateShellManager();
	}
}