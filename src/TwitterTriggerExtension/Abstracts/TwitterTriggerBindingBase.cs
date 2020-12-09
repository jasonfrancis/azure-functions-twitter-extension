using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;

namespace TwitterTriggerExtension
{
	public abstract class TwitterTriggerBindingBase<T, E> : ITriggerBinding where T : TwitterTriggerAttributeBase
	{
		private readonly string _triggerName;
		private readonly ParameterInfo _parameter;
        protected readonly ILogger _logger;
        protected readonly T _attribute;
        private readonly IReadOnlyDictionary<string, Type> _bindingContract;

        protected TwitterTriggerBindingBase(ParameterInfo parameter, ILogger logger, string triggerName)
        {
            _parameter = parameter;
            _logger = logger;
            _triggerName = triggerName;
            _attribute = parameter.GetCustomAttribute<T>(inherit: false);
            _bindingContract = CreateBindingContract();
        }

		public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingContract;
		public Type TriggerValueType => typeof(E);

		private IReadOnlyDictionary<string, Type> CreateBindingContract()
        {
            if(string.IsNullOrWhiteSpace(_triggerName))
            {
                throw new Exception("_triggerName cannot be null or empty when extending TwitterTriggerBindingBase");
            }
            Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            contract.Add(_triggerName, typeof(E));
            return contract;
        }

		public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            E tweetEvent = (E)Convert.ChangeType(value, typeof(E));
			if (tweetEvent == null)
            {
                string tweetInfo = value as string;
                tweetEvent = GetEventArgsFromString(tweetInfo);
            }

            IReadOnlyDictionary<string, object> bindingData = GetBindingData(tweetEvent);

            return Task.FromResult<ITriggerData>(new TriggerData(null, bindingData));
        }

		private IReadOnlyDictionary<string, object> GetBindingData(E value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Dictionary<string, object> bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            bindingData.Add(_triggerName, value);

            return bindingData;
        }

		internal static E GetEventArgsFromString(string tweetInfo)
        {
            if (!string.IsNullOrEmpty(tweetInfo))
            {
                //TODO: Figure this out: https://github.com/Azure/azure-webjobs-sdk-extensions/blob/master/src/WebJobs.Extensions/Extensions/Files/Bindings/FileTriggerBinding.cs
            }

            return default(E);
        }

		public abstract Task<IListener> CreateListenerAsync(ListenerFactoryContext context);
        /*
        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return Task.FromResult<IListener>(new TwitterListener(context.Executor, _attribute));
        }
        */

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