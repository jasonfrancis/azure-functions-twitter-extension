using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Tweetinvi.Events;
using Tweetinvi.Events.V2;

namespace TwitterTriggerExtension
{
    public class TwitterTriggerV2AttributeBindingProvider : TwitterTriggerAttributeBindingProviderBase<TwitterTriggerV2Attribute, FilteredStreamTweetV2EventArgs>
    {
        public TwitterTriggerV2AttributeBindingProvider(ILoggerFactory loggerFactory) : base(loggerFactory){}

		protected override ITriggerBinding GetTriggerBinding(ParameterInfo parameter, ILogger logger)
		{
			return new TwitterTriggerV2Binding(parameter, logger);
		}
	}
}
