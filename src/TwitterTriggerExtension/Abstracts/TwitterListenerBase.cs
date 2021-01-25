using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Tweetinvi.Streaming;

namespace TwitterTriggerExtension
{
    public abstract class TwitterListenerBase<A, T> : IListener where A : TwitterTriggerAttributeBase
    {
		public ITriggeredFunctionExecutor Executor { get; }

        protected A _attribute;
        protected T _filteredStream;

        public TwitterListenerBase(ITriggeredFunctionExecutor executor, A attribute)
        {
            Executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        }

        public abstract void Cancel();

        public virtual void Dispose() => Cancel();

        public abstract Task StartAsync(CancellationToken cancellationToken);

        public abstract Task StopAsync(CancellationToken cancellationToken);
	}
}