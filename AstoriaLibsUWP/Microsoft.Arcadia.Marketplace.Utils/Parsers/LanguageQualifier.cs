using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Arcadia.Marketplace.Utils.Log;

namespace Microsoft.Arcadia.Marketplace.Utils.Parsers
{
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Tool reports error on some abbreviations such as en-US")]
	public static class LanguageQualifier
	{
		private static readonly HashSet<string> AllSupportedLanguageQualifiers = new HashSet<string>
	{
		"AF", "AF-ZA", "AM", "AM-ET", "AR", "AR-AE", "AR-BH", "AR-DZ", "AR-EG", "AR-IQ",
		"AR-JO", "AR-KW", "AR-LB", "AR-LY", "AR-MA", "AR-OM", "AR-QA", "AR-SA", "AR-SY", "AR-TN",
		"AR-YE", "AS", "AS-IN", "AZ-ARAB", "AZ-ARAB-AZ", "AZ-CYRL", "AZ-CYRL-AZ", "AZ-LATN", "AZ-LATN-AZ", "BE",
		"BE-BY", "BG", "BG-BG", "BN", "BN-BD", "BN-IN", "BS", "BS-CYRL", "BS-CYRL-BA", "BS-LATN",
		"BS-LATN-BA", "CA", "CA-ES", "CA-ES-VALENCIA", "CHR-CHER", "CHR-CHER-US", "CHR-LATN", "CS", "CS-CZ", "CY",
		"CY-GB", "DA", "DA-DK", "DE", "DE-AT", "DE-CH", "DE-DE", "DE-LI", "DE-LU", "EL",
		"EL-GR", "EN", "EN-011", "EN-014", "EN-018", "EN-021", "EN-029", "EN-053", "EN-AU", "EN-BZ",
		"EN-CA", "EN-GB", "EN-HK", "EN-ID", "EN-IE", "EN-IN", "EN-JM", "EN-KZ", "EN-MT", "EN-MY",
		"EN-NZ", "EN-PH", "EN-PK", "EN-SG", "EN-TT", "EN-US", "EN-VN", "EN-ZA", "ES", "ES-019",
		"ES-419", "ES-AR", "ES-BO", "ES-CL", "ES-CO", "ES-CR", "ES-DO", "ES-EC", "ES-ES", "ES-GT",
		"ES-HN", "ES-MX", "ES-NI", "ES-PA", "ES-PE", "ES-PR", "ES-PY", "ES-SV", "ES-US", "ES-UY",
		"ES-VE", "ET", "ET-EE", "EU", "EU-ES", "FA", "FA-IR", "FI", "FI-FI", "FIL",
		"FIL-LATN", "FIL-PH", "FR", "FR-011", "FR-015", "FR-021", "FR-029", "FR-155", "FR-BE", "FR-CA",
		"FR-CD", "FR-CH", "FR-CI", "FR-CM", "FR-FR", "FR-HT", "FR-LU", "FR-MA", "FR-MC", "FR-ML",
		"FR-RE", "FRC-LATN", "FRP-LATN", "GA", "GA-IE", "GD-GB", "GD-LATN", "GL", "GL-ES", "GU",
		"GU-IN", "HA-LATN", "HA-LATN-NG", "HE", "HE-IL", "HI", "HI-IN", "HR", "HR-BA", "HR-HR",
		"HU", "HU-HU", "HY", "HY-AM", "ID", "ID-ID", "IG-LATN", "IG-NG", "IS", "IS-IS",
		"IT", "IT-CH", "IT-IT", "IU-CANS", "IU-LATN", "IU-LATN-CA", "JA", "JA-JP", "KA", "KA-GE",
		"KK", "KK-KZ", "KM", "KM-KH", "KN", "KN-IN", "KO", "KO-KR", "KOK", "KOK-IN",
		"KU-ARAB", "KU-ARAB-IQ", "KY-CYRL", "KY-KG", "LB", "LB-LU", "LT", "LT-LT", "LV", "LV-LV",
		"MI", "MI-LATN", "MI-NZ", "MK", "MK-MK", "ML", "ML-IN", "MN-CYRL", "MN-MN", "MN-MONG",
		"MN-PHAG", "MR", "MR-IN", "MS", "MS-BN", "MS-MY", "MT", "MT-MT", "NB", "NB-NO",
		"NE", "NE-NP", "NL", "NL-BE", "NL-NL", "NN", "NN-NO", "NO", "NO-NO", "NSO",
		"NSO-ZA", "OR", "OR-IN", "PA", "PA-ARAB", "PA-ARAB-PK", "PA-DEVA", "PA-IN", "PL", "PL-PL",
		"PRS", "PRS-AF", "PRS-ARAB", "PT", "PT-BR", "PT-PT", "QUC-LATN", "QUT-GT", "QUT-LATN", "QUZ",
		"QUZ-BO", "QUZ-EC", "QUZ-PE", "RO", "RO-RO", "RU", "RU-RU", "RW", "RW-RW", "SD-ARAB",
		"SD-ARAB-PK", "SD-DEVA", "SI", "SI-LK", "SK", "SK-SK", "SL", "SL-SI", "SQ", "SQ-AL",
		"SR-CYRL", "SR-CYRL-BA", "SR-CYRL-CS", "SR-CYRL-ME", "SR-CYRL-RS", "SR-LATN", "SR-LATN-BA", "SR-LATN-CS", "SR-LATN-ME", "SR-LATN-RS",
		"SV", "SV-FI", "SV-SE", "SW", "SW-KE", "TA", "TA-IN", "TE", "TE-IN", "TG-ARAB",
		"TG-CYRL", "TG-CYRL-TJ", "TG-LATN", "TH", "TH-TH", "TI", "TI-ET", "TK-CYRL", "TK-CYRL-TR", "TK-LATN",
		"TK-LATN-TR", "TK-TM", "TN", "TN-BW", "TN-ZA", "TR", "TR-TR", "TT-ARAB", "TT-CYRL", "TT-LATN",
		"TT-RU", "UG-ARAB", "UG-CN", "UG-CYRL", "UG-LATN", "UK", "UK-UA", "UR", "UR-PK", "UZ-CYRL",
		"UZ-LATN", "UZ-LATN-UZ", "VI", "VI-VN", "WO", "WO-SN", "XH", "XH-ZA", "YO-LATN", "YO-NG",
		"ZH-CN", "ZH-HANS", "ZH-HANS-CN", "ZH-HANS-SG", "ZH-HANT", "ZH-HANT-HK", "ZH-HANT-MO", "ZH-HANT-TW", "ZH-HK", "ZH-TW",
		"ZU", "ZU-ZA"
	};

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "We validate language qualifier by creating a CultureInfo instance")]
		public static bool IsValidLanguageQualifier(string languageQualifier)
		{
			if (string.IsNullOrWhiteSpace(languageQualifier))
			{
				throw new ArgumentException("A valid language qualifier must be provided.", "languageQualifier");
			}
			if (!AllSupportedLanguageQualifiers.Contains(languageQualifier.ToUpper()))
			{
				LoggerCore.Log("{0} is not a whitelisted language qualifier.", languageQualifier);
				return false;
			}
			try
			{
				new CultureInfo(languageQualifier);
			}
			catch (CultureNotFoundException)
			{
				return false;
			}
			return true;
		}
	}
}