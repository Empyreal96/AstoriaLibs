using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal sealed class AdbMessageStrings
	{
		public const string SuccessPackageInstalled = "Success";

		public const string SuccessPackageUninstalled = "Success";

		public const string SuccessIntentStarted = "Success";

		public const string FailureGenericError = "Failure";

		public const string FailureInternal = "Failure [INTERNAL_AGENT_ERROR]";

		public const string FailureDecoderManifestError = "Failure [MANIFEST_DECODER_ERROR]";

		public const string FailureDecoderManifestApplicationElementError = "Failure [MANIFEST_DECODER_ERROR_APPLICATION_ELEMENT_NOT_FOUND]";

		public const string FailureDecoderManifestVersionCodeError = "Failure [MANIFEST_DECODER_ERROR_VERSION_NOT_FOUND]";

		public const string FailureDecoderResourcesError = "Failure [RESOURCES_DECODER_ERROR]";

		public const string FailureDecoderConfigError = "Failure [CONFIGURE_DECODER_ERROR]";

		public const string FailureConverterError = "Failure [CONVERTER_ERROR]";

		public const string FailurePackageInstallNotRegisteredError = "Failure [PACKAGE_INSTALL_REGISTRATION_ERROR]";

		public const string FailureDeveloperLocked = "Failure [DEVICE_NOT_DEVELOPER_UNLOCKED]";

		public const string FailureExceededDeployLimit = "Failure [EXCEEDED_APP_DEPLOY_LIMIT]";

		public const string FailureOutOfDiskSpace = "Failure [OUT_OF_DISK_SPACE]";

		public const string FailureCantDeletingExistingAppData = "Failure [DELETING_EXISTING_APPLICATIONDATA_STORE_FAILED]";

		public const string FailurePackageAlreadyInstalled = "Failure [INSTALL_FAILED_ALREADY_EXISTS]";

		public const string FailureInvalidApk = "Failure [INSTALL_FAILED_INVALID_APK]";

		public const string FailureInstallInvalidUri = "Failure [INSTALL_FAILED_INVALID_URI]";

		public const string FailureInsufficientStorage = "Failure [INSTALL_FAILED_INSUFFICIENT_STORAGE]";

		public const string FailureDuplicatePackage = "Failure [INSTALL_FAILED_DUPLICATE_PACKAGE]";

		public const string FailureNoSharedUser = "Failure [INSTALL_FAILED_NO_SHARED_USER]";

		public const string FailureUpateIncompatible = "Failure [INSTALL_FAILED_UPDATE_INCOMPATIBLE]";

		public const string FailureSharedUserIncompatible = "Failure [INSTALL_FAILED_SHARED_USER_INCOMPATIBLE]";

		public const string FailureMissingSharedLibrary = "Failure [INSTALL_FAILED_MISSING_SHARED_LIBRARY]";

		public const string FailureReplaceCouldntDelete = "Failure [INSTALL_FAILED_REPLACE_COULDNT_DELETE]";

		public const string FailureDexOpt = "Failure [INSTALL_FAILED_DEXOPT]";

		public const string FailureOlderSdk = "Failure [INSTALL_FAILED_OLDER_SDK]";

		public const string FailureConflictingProvider = "Failure [INSTALL_FAILED_CONFLICTING_PROVIDER]";

		public const string FailureNewerSdk = "Failure [INSTALL_FAILED_NEWER_SDK]";

		public const string FailureTestOnly = "Failure [INSTALL_FAILED_TEST_ONLY]";

		public const string FailureCpuAbiIncompatible = "Failure [INSTALL_FAILED_CPU_ABI_INCOMPATIBLE]";

		public const string FailureMissingFeature = "Failure [INSTALL_FAILED_MISSING_FEATURE]";

		public const string FailureContainerError = "Failure [INSTALL_FAILED_CONTAINER_ERROR]";

		public const string FailureInvalidInstallLocation = "Failure [INSTALL_FAILED_INVALID_INSTALL_LOCATION]";

		public const string FailureMediaUnavailable = "Failure [INSTALL_FAILED_MEDIA_UNAVAILABLE]";

		public const string FailureVerificationTimeout = "Failure [INSTALL_FAILED_VERIFICATION_TIMEOUT]";

		public const string FailureVerificationFailure = "Failure [INSTALL_FAILED_VERIFICATION_FAILURE]";

		public const string FailurePackageChanged = "Failure [INSTALL_FAILED_PACKAGE_CHANGED]";

		public const string FailureUidChanged = "Failure [INSTALL_FAILED_UID_CHANGED]";

		public const string FailureVersionDowngrade = "Failure [INSTALL_FAILED_VERSION_DOWNGRADE]";

		public const string FailureNotApk = "Failure [INSTALL_PARSE_FAILED_NOT_APK]";

		public const string FailureBadManifest = "Failure [INSTALL_PARSE_FAILED_BAD_MANIFEST]";

		public const string FailureParseUnexpectedException = "Failure [INSTALL_PARSE_FAILED_UNEXPECTED_EXCEPTION]";

		public const string FailureNoCertificates = "Failure [INSTALL_PARSE_FAILED_NO_CERTIFICATES]";

		public const string FailureInconsistentCertificates = "Failure [INSTALL_PARSE_FAILED_INCONSISTENT_CERTIFICATES]";

		public const string FailureCertificatesEncoding = "Failure [INSTALL_PARSE_FAILED_CERTIFICATE_ENCODING]";

		public const string FailureBadPackageName = "Failure [INSTALL_PARSE_FAILED_BAD_PACKAGE_NAME]";

		public const string FailureBadSharedUserId = "Failure [INSTALL_PARSE_FAILED_BAD_SHARED_USER_ID]";

		public const string FailureManifestMalformed = "Failure [INSTALL_PARSE_FAILED_MANIFEST_MALFORMED]";

		public const string FailureManifestEmpty = "Failure [INSTALL_PARSE_FAILED_MANIFEST_EMPTY]";

		public const string FailureInstallInternalError = "Failure [INSTALL_FAILED_INTERNAL_ERROR]";

		public const string FailureUserRestricted = "Failure [INSTALL_FAILED_USER_RESTRICTED]";

		public const string FailurePackageUriPrefixMissing = "Failure [MISSING_PACKAGE_URI]";

		public const string FailurePackageUninstallError = "Failure [PACKAGE_UNINSTALL_ERROR]";

		public const string FailurePackageNotFound = "Failure [PACKAGE_NOT_FOUND]";

		public const string FailurePackageInvalidName = "Failure [PACKAGE_INVALID_NAME]";

		public const string FailurePackageAmbiguousName = "Failure [PACKAGE_AMBIGUOUS]";

		public const string FailureNoIntentSpecified = "Failure [INTENT_NOT_SPECIFIED]";

		public const string FailureUnsupportedIntent = "Failure [INTENT_NOT_SUPPORTED]";

		public const string FailureIntentStartError = "Failure [INTENT_START_ERROR]";

		public const string FailurePackageUriCorrupted = "Failure [MALFORMED_PACKAGE_URI]";

		public const string FailureDeviceLocked = "Failure [SCREEN_LOCKED]";

		private const uint ErrorInstallRegistrationFailure = 2147958006u;




		public static string FromAndroidUninstallResult(AndroidPackageUninstallResult result)
		{
            switch (result)
            {
				case AndroidPackageUninstallResult.Success:
					return "Success";
				case AndroidPackageUninstallResult.UninstallError:
					return "Failure [PACKAGE_UNINSTALL_ERROR]";
				case AndroidPackageUninstallResult.NotFound:
					return "Failure [PACKAGE_NOT_FOUND]";
				case AndroidPackageUninstallResult.AmbiguousPackage:
					return "Failure [PACKAGE_AMBIGUOUS]";
				default:
					return "Failure [PACKAGE_UNINSTALL_ERROR]";


			}
		}

		public static string FromPackageManagerInstallResult(PackageDeploymentResult result)
		{
			if (result.Error == null)
			{
				return "Success";
			}
			string text = null;
			int hResult = result.Error.HResult;
			if (hResult == -2147009290 && result.ExtendedError != null && result.ExtendedError.HResult != 0)
			{
				hResult = result.ExtendedError.HResult;
			}
			text = GetAndroidPackageManagerErrorString(hResult);
			if (string.IsNullOrWhiteSpace(text))
			{
				text = GetWindowsPackageManagerErrorString(hResult);
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				text = GenerateUnknownInstallErrorString(hResult);
			}
			return text;
		}

		private static string GetWindowsPackageManagerErrorString(int errorCode)
		{
			switch (errorCode)
			{
				case -2147009285:
				case -2130509513:
					return "Failure [INSTALL_FAILED_ALREADY_EXISTS]";
				case -2147009281:
					return "Failure [DEVICE_NOT_DEVELOPER_UNLOCKED]";
				case -2130509543:
				case 15633:
					return "Failure [EXCEEDED_APP_DEPLOY_LIMIT]";
				case -2147009275:
					return "Failure [DELETING_EXISTING_APPLICATIONDATA_STORE_FAILED]";
				case -2147009290:
					return "Failure [PACKAGE_INSTALL_REGISTRATION_ERROR]";
				default:
					return null;
			}
		}

		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Although it's a big switch case, the logic is simple.")]
		private static string GetAndroidPackageManagerErrorString(int errorCode)
		{
			switch(errorCode)
            {
				case -1:
					return "Failure [INSTALL_FAILED_ALREADY_EXISTS]";
				case -2:
						return "Failure [INSTALL_FAILED_INVALID_APK]";
				case -3:
						return "Failure [INSTALL_FAILED_INVALID_URI]";
				case -4:
						return "Failure [INSTALL_FAILED_INSUFFICIENT_STORAGE]";
				case -5:
						return "Failure [INSTALL_FAILED_DUPLICATE_PACKAGE]";
				case -6:
						return "Failure [INSTALL_FAILED_NO_SHARED_USER]";
				case -7:
						return "Failure [INSTALL_FAILED_UPDATE_INCOMPATIBLE]";
				case -8:
						return "Failure [INSTALL_FAILED_SHARED_USER_INCOMPATIBLE]";
				case -9:
						return "Failure [INSTALL_FAILED_MISSING_SHARED_LIBRARY]";
				case -10: 
						return "Failure [INSTALL_FAILED_REPLACE_COULDNT_DELETE]";
				case -11:
						return "Failure [INSTALL_FAILED_DEXOPT]";
				case -12:
						return "Failure [INSTALL_FAILED_OLDER_SDK]";
				case -13:
						return "Failure [INSTALL_FAILED_CONFLICTING_PROVIDER]";
				case -14:
						return "Failure [INSTALL_FAILED_NEWER_SDK]";
				case -15:
						return "Failure [INSTALL_FAILED_TEST_ONLY]";
				case -16:
						return "Failure [INSTALL_FAILED_CPU_ABI_INCOMPATIBLE]";
				case -17:
						return "Failure [INSTALL_FAILED_MISSING_FEATURE]";
				case -18:
						return "Failure [INSTALL_FAILED_CONTAINER_ERROR]";
				case -19:
						return "Failure [INSTALL_FAILED_INVALID_INSTALL_LOCATION]";
				case -20:
						return "Failure [INSTALL_FAILED_MEDIA_UNAVAILABLE]";
				case -21:
						return "Failure [INSTALL_FAILED_VERIFICATION_TIMEOUT]";
				case -22:
						return "Failure [INSTALL_FAILED_VERIFICATION_FAILURE]";
				case -23:
						return "Failure [INSTALL_FAILED_PACKAGE_CHANGED]";
				case -24:
						return "Failure [INSTALL_FAILED_UID_CHANGED]";
				case -25:
						return "Failure [INSTALL_FAILED_VERSION_DOWNGRADE]";
				case -100: 
						return "Failure [INSTALL_PARSE_FAILED_NOT_APK]";
				case -101:
						return "Failure [INSTALL_PARSE_FAILED_BAD_MANIFEST]";
				case -102:
						return "Failure [INSTALL_PARSE_FAILED_UNEXPECTED_EXCEPTION]";
				case -103:
						return "Failure [INSTALL_PARSE_FAILED_NO_CERTIFICATES]";
				case -104:
						return "Failure [INSTALL_PARSE_FAILED_INCONSISTENT_CERTIFICATES]";
				case -105:
						return "Failure [INSTALL_PARSE_FAILED_CERTIFICATE_ENCODING]";
				case -106:
						return "Failure [INSTALL_PARSE_FAILED_BAD_PACKAGE_NAME]";
				case -107:
						return "Failure [INSTALL_PARSE_FAILED_BAD_SHARED_USER_ID]";
				case -108:
						return "Failure [INSTALL_PARSE_FAILED_MANIFEST_MALFORMED]";
				case -109:
						return "Failure [INSTALL_PARSE_FAILED_MANIFEST_EMPTY]";
				case -110:
						return "Failure [INSTALL_FAILED_INTERNAL_ERROR]";
				default:
					return null;
            }
		}

		private static string GenerateUnknownInstallErrorString(int errorCode)
		{
			return "Failure [INSTALL_FAILED_(" + errorCode + ")]";
		}
	}
}
