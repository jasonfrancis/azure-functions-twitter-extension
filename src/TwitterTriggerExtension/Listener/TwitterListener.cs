using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace TwitterTriggerExtension
{
    public class TwitterListener : TwitterListenerBase<TwitterTriggerAttribute, IFilteredStream>
    {
        public TwitterListener(ITriggeredFunctionExecutor executor, TwitterTriggerAttribute attribute) : base(executor, attribute) {}

		public override void Cancel()
		{
			_filteredStream.Stop();
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var consumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey");
            var consumerSecret = Environment.GetEnvironmentVariable("TwitterConsumerSecret");
            var accessKey = Environment.GetEnvironmentVariable("TwitterAccessKey");
            var accessSecret = Environment.GetEnvironmentVariable("TwitterAccessSecret");

            var credentials = new TwitterCredentials(consumerKey, consumerSecret, accessKey, accessSecret);
            var client = new TwitterClient(credentials);

            _filteredStream = client.Streams.CreateFilteredStream();

            if (!string.IsNullOrEmpty(_attribute.Filter))
            {
                _filteredStream.AddTrack(_attribute.Filter);
            }

            if (!string.IsNullOrWhiteSpace(_attribute.User))
            {
                _filteredStream.AddFollow(await client.Users.GetUserAsync(_attribute.User));
            }

            _filteredStream.MatchingTweetReceived += async (obj, tweetEvent) =>
            {
                var triggerData = new TriggeredFunctionData
                {
                    TriggerValue = tweetEvent
                };
                await Executor.TryExecuteAsync(triggerData, CancellationToken.None);
            };

            _filteredStream.DisconnectMessageReceived += (obj, disconnectEvent) =>
            {
                _filteredStream.Stop();
            };

            await _filteredStream.StartMatchingAllConditionsAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _filteredStream.Stop();
            return Task.CompletedTask;
        }
	}
}
