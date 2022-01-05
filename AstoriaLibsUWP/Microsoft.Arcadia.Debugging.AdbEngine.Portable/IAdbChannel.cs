using System.Diagnostics.CodeAnalysis;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Tool reports error on some abbreviations such as arg, ls")]
	public interface IAdbChannel
	{
		IStreamReader StreamReader { get; }

		IStreamWriter StreamWriter { get; }
	}
}
