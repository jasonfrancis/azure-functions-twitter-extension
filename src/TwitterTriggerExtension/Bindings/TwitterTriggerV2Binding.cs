using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using Tweetinvi.Events;
using Tweetinvi.Events.V2;

namespace TwitterTriggerExtension
{
	internal class TwitterTriggerV2Binding : ITriggerBinding
	{
		private readonly ParameterInfo _parameter;
        private readonly ILogger _logger;
        private readonly TwitterTriggerV2Attribute _attribute;
        private readonly IReadOnlyDictionary<string, Type> _bindingContract;

        public TwitterTriggerV2Binding(ParameterInfo parameter, ILogger logger)
        {
            _parameter = parameter;
            _logger = logger;
            _attribute = parameter.GetCustomAttribute<TwitterTriggerV2Attribute>(inherit: false);
            _bindingContract = CreateBindingContract();
        }
        public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingContract;
        public Type TriggerValueType => typeof(FilteredStreamTweetV2EventArgs);

        private IReadOnlyDictionary<string, Type> CreateBindingContract()
        {
            Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            contract.Add("TwitterTriggerV2", typeof(FilteredStreamTweetV2EventArgs));
            return contract;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            FilteredStreamTweetV2EventArgs tweetEvent = value as FilteredStreamTweetV2EventArgs;
            if (tweetEvent == null)
            {
                string tweetInfo = value as string;
                tweetEvent = GetEventArgsFromString(tweetInfo);
            }

            IReadOnlyDictionary<string, object> bindingData = GetBindingData(tweetEvent);

            return Task.FromResult<ITriggerData>(new TriggerData(null, bindingData));
        }

        private IReadOnlyDictionary<string, object> GetBindingData(FilteredStreamTweetV2EventArgs value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Dictionary<string, object> bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            bindingData.Add("TwitterTriggerV2", value);

            return bindingData;
        }

        internal static FilteredStreamTweetV2EventArgs GetEventArgsFromString(string tweetInfo)
        {
            if (!string.IsNullOrEmpty(tweetInfo))
            {
                //TODO: Figure this out: https://github.com/Azure/azure-webjobs-sdk-extensions/blob/master/src/WebJobs.Extensions/Extensions/Files/Bindings/FileTriggerBinding.cs
            }

            return null;
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return Task.FromResult<IListener>(new TwitterV2Listener(context.Executor, _attribute));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            string filter = _attribute.Filter;
            string user = _attribute.User;

            return new TwitterTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "Enter a string to filter on",
                    Description = $"Tweet event occured on filter text {filter}",
                    DefaultValue = "#azure"
                }
            };
        }

        private class TwitterTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                if (arguments != null && arguments.TryGetValue(Name, out var filter))
                {
                    return $"Tweet detected at {DateTime.Now.ToString("o")}";
                }
                return null;
            }
        }
	}
}