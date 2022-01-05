namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal sealed class AdbRegularExpressions
	{
		public const string ExplicitIntentActivityRegex = "^([a-z0-9\\._]+)/([a-z0-9\\._]+)$";

		public const string PackageNameRegex = "^([a-z0-9\\._]+)$";
	}
}
