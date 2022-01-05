using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ReportBuilderInfoAttribute : Attribute
	{
		public string Owner { get; private set; }

		public string MessagePrefix { get; private set; }

		public ReportBuilderInfoAttribute(string owner, string messagePrefix)
		{
			if (string.IsNullOrWhiteSpace(owner))
			{
				throw new ArgumentException("Owner must not be null or empty.", "owner");
			}
			if (string.IsNullOrWhiteSpace(messagePrefix))
			{
				throw new ArgumentException("messagePrefix must not be null or empty.", "messagePrefix");
			}
			Owner = owner;
			MessagePrefix = messagePrefix.ToUpperInvariant();
		}
	}
}