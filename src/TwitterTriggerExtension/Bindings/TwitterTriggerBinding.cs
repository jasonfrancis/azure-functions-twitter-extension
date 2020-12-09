using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Tweetinvi.Events;
using Tweetinvi.Events.V2;

namespace TwitterTriggerExtension
{
	internal class TwitterTriggerBinding : TwitterTriggerBindingBase<TwitterTriggerAttribute, MatchedTweetReceivedEventArgs>
	{
		public TwitterTriggerBinding(ParameterInfo parameter, ILogger logger) : base(parameter, logger, "TwitterTrigger") { }

		public override Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
		{
			if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return Task.FromResult<IListener>(new TwitterListener(context.Executor, _attribute));
		}
	}

    internal class TwitterTriggerV2Binding : TwitterTriggerBindingBase<TwitterTriggerV2Attribute, FilteredStreamTweetV2EventArgs>
	{
        public TwitterTriggerV2Binding(ParameterInfo parameter, ILogger logger) : base(parameter, logger, "TwitterTriggerV2") { }

		public override Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
		{
			if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return Task.FromResult<IListener>(new TwitterV2Listener(context.Executor, _attribute, _logger));
		}
	}
}