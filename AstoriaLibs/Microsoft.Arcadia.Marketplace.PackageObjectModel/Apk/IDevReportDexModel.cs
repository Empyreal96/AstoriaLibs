using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
	[ComVisible(false)]
	public interface IDevReportDexModel
	{
		IReadOnlyCollection<string> FindAllClassesExtendingGivenBase(string baseClass);

		IReadOnlyCollection<string> FindAllClassesExtendingAnyGivenBases(IReadOnlyCollection<string> baseClasses);

		IReadOnlyCollection<string> FindAllClassesImplementingAnyGivenInterfaces(IReadOnlyCollection<string> interfaces);

		IReadOnlyCollection<string> FindAllClassesWithMethodCall(string methodSearchTerm);

		IReadOnlyCollection<string> FindAllClassesWithMethodCall(IEnumerable<string> methodSearchTerms);

		IReadOnlyCollection<string> FindDirectAndIndirectCallsToBaseClassMethod(string childClassName, string baseClassName, string baseClassMethodName);

		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Purposefully encapsulated to allow ease of review by external stake holders.")]
		IReadOnlyCollection<DevReportDexMethodInvocation> FindAllMethodCallersForMethodInvocationWithParameters(string methodSearchTerm, IList<KeyValuePair<int, string>> parametersAndPositions, bool successOnAnyParameter);

		IReadOnlyCollection<string> FindStringValuesOfParameterAtMethodCall(IList<string> methodCalls, int parameterPosition);

		IReadOnlyCollection<DevReportDexMethodInvocation> FindAllMethodInvocations(string methodName);

		IReadOnlyCollection<DevReportDexMethodInvocation> FindAllMethodInvocations(IList<string> methodNames);

		IReadOnlyCollection<string> FindAllDirectAndIndirectChildrenOfGivenBase(string baseClassName);

		IReadOnlyCollection<string> FindAllDirectAndIndirectChildrenOfAnyGivenBase(IReadOnlyCollection<string> baseClassNames);

		IReadOnlyCollection<string> RetrieveAllDexSignatures(IReadOnlyCollection<string> trackedSdkList);
	}
}