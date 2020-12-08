using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;

namespace TwitterTriggerExtension
{
    public abstract class TwitterTriggerAttributeBindingProviderBase<A, E> : ITriggerBindingProvider where A : TwitterTriggerAttributeBase 
    {
		protected readonly ILogger _logger;

        public TwitterTriggerAttributeBindingProviderBase(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger(LogCategories.CreateTriggerCategory("Twitter"));
        }

		protected abstract ITriggerBinding GetTriggerBinding(ParameterInfo parameter, ILogger logger);

		public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }
            //Retrieve Parameter
            ParameterInfo parameter = context.Parameter;
            A attribute = parameter.GetCustomAttribute<A>(inherit: false);
            if(attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }
            //Validate Trigger
            if (!IsSupportedBindingType(parameter.ParameterType))
            {
                throw new InvalidOperationException($"Can't bind TwitterTriggerAttribute to type '{parameter.ParameterType}'");
            }
            return Task.FromResult<ITriggerBinding>(GetTriggerBinding(parameter, _logger));
        }

        private bool IsSupportedBindingType(Type t)
        {
            return t == typeof(E);
        }
	}
}