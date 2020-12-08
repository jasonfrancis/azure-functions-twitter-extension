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
	internal class TwitterTriggerV2Binding : TwitterTriggerBindingBase<TwitterTriggerV2Attribute, FilteredStreamTweetV2EventArgs>
	{
        public TwitterTriggerV2Binding(ParameterInfo parameter, ILogger logger) : base(parameter, logger, "TwitterTriggerV2") { }

		public override Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
		{
			if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return Task.FromResult<IListener>(new TwitterV2Listener(context.Executor, _attribute));
		}
	}
}