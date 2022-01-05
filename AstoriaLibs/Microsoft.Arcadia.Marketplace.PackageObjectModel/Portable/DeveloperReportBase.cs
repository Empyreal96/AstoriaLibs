using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
	[DataContract]
	public abstract class DeveloperReportBase : IDeveloperReport
	{
		private const string ErrorMessagePrefix = "ER";

		private const string InfoMessagePrefix = "IN";

		private const string WarningMessagePrefix = "WA";

		private HashSet<Enum> alreadyReportedMessages = new HashSet<Enum>();

		public static string GetMessagePrefix(string builderPrefix, WorkerLogLevel logLevel, Enum messageId)
		{
			if (string.IsNullOrEmpty("builderPrefix"))
			{
				throw new ArgumentException("buildPrefix cannot be null or empty.", "builderPrefix");
			}
			if (messageId == null)
			{
				throw new ArgumentNullException("messageId");
			}
			string text = "IN";
			switch (logLevel)
			{
				case WorkerLogLevel.Error:
					text = "ER";
					break;
				case WorkerLogLevel.Warning:
					text = "WA";
					break;
			}
			return text + builderPrefix + messageId.ToString("X").Substring(4);
		}

		public void AddReportMessage(IFeatureDetails featureDetails)
		{
			if (featureDetails == null)
			{
				throw new ArgumentNullException("featureDetails");
			}
			IFeatureLog feature = featureDetails.CreateFeatureLog();
			OnAddReportMessage(feature);
			AggregateFeature aggregateFeature = featureDetails.CreateAggregateFeature();
			if (aggregateFeature != null)
			{
				OnAddAggregateFeature(aggregateFeature.AggregateFeatureName, aggregateFeature.MessageVersion);
			}
			alreadyReportedMessages.Add(featureDetails.Message);
		}

		public void AddNewReportMessage(IFeatureDetails featureDetails)
		{
			if (featureDetails == null)
			{
				throw new ArgumentNullException("featureDetails");
			}
			if (!alreadyReportedMessages.Contains(featureDetails.Message))
			{
				AddReportMessage(featureDetails);
			}
		}

		protected abstract void OnAddReportMessage(IFeatureLog feature);

		protected abstract void OnAddAggregateFeature(string aggregateFeature, uint messageVersion);

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			alreadyReportedMessages = new HashSet<Enum>();
		}
	}
}