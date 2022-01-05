using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
	internal class Intent
	{
		public const string ActionDelete = "android.intent.action.DELETE";

		public bool IsExplicitIntent
		{
			get
			{
				if (PackageName != null)
				{
					return ActivityName != null;
				}
				return false;
			}
		}

		public bool IsUnsupportedIntent => !IsExplicitIntent;

		public string Action { get; set; }

		public string Category { get; set; }

		public string PackageName { get; set; }

		public string ActivityName { get; set; }

		public Uri DataUri { get; set; }

		public bool HasDataFlag => DataUri != null;
	}
}