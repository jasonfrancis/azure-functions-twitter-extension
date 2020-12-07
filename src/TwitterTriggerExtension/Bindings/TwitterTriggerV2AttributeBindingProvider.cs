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
    public class TwitterTriggerV2AttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly ILogger _logger;

        public TwitterTriggerV2AttributeBindingProvider(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger(LogCategories.CreateTriggerCategory("Twitter"));
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }

            //Retrieve Parameter
            ParameterInfo parameter = context.Parameter;
            TwitterTriggerV2Attribute attribute = parameter.GetCustomAttribute<TwitterTriggerV2Attribute>(inherit: false);
            if(attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            //Validate Trigger
            if (!IsSupportedBindingType(parameter.ParameterType))
            {
                throw new InvalidOperationException($"Can't bind TwitterTriggerV2Attribute to type '{parameter.ParameterType}'");
            }

            return Task.FromResult<ITriggerBinding>(new TwitterTriggerV2Binding(parameter, _logger));
        }

        private bool IsSupportedBindingType(Type t)
        {
            return t == typeof(FilteredStreamTweetV2EventArgs);
        }
    }
}
