using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;
using Tweetinvi.Streaming.V2;

namespace TwitterTriggerExtension
{
	public class TwitterV2Listener : TwitterListenerBase<TwitterTriggerV2Attribute, IFilteredStreamV2>
	{
		public TwitterV2Listener(ITriggeredFunctionExecutor executor, TwitterTriggerV2Attribute attribute) : base(executor, attribute)
		{
		}

		public override void Cancel()
		{
			if(_filteredStream != null)
			{
				_filteredStream.StopStream();
			}
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			var v2ConsumerKey = Environment.GetEnvironmentVariable("TwitterV2ConsumerKey");
			var v2ConsumerSecret = Environment.GetEnvironmentVariable("TwitterV2ConsumerSecret");
			var v2BearerToken = Environment.GetEnvironmentVariable("TwitterV2BearerToken");

			var credentials = new ConsumerOnlyCredentials(v2ConsumerKey, v2ConsumerSecret, v2BearerToken);
			var client = new TwitterClient(credentials);

			// Generate our filter string.
			var filterString = _attribute.Filter;

			// The V2 API listens to ScreenNames a little differently
			// if we were given a User to watch update filterString to recreate the behavior of the V1.1 API.
			if(string.IsNullOrWhiteSpace(_attribute.User) == false)
			{
				filterString += $" (from:{_attribute.User} OR to:{_attribute.User} OR @{_attribute.User} OR retweets_of:{_attribute.User})";
				filterString = filterString.Trim();
			}

			// Clear out the existing filter rules.
			var existingRules = await client.StreamsV2.GetRulesForFilteredStreamV2Async();
			HandleFilteredStreamRulesResponseErrors(existingRules);

			if(existingRules.Rules.Count() > 0)
			{
				var ruleIds = existingRules.Rules.Select(r => r.Id).ToArray();
				var delResponse = await client.StreamsV2.DeleteRulesFromFilteredStreamAsync(ruleIds);
				HandleFilteredStreamRulesResponseErrors(delResponse);
			}

			// Set our own filter rules.
			var rule = new FilteredStreamRuleConfig(filterString);
			var addRulesResponse = await client.StreamsV2.AddRulesToFilteredStreamAsync(rule);
			HandleFilteredStreamRulesResponseErrors(addRulesResponse);

			_filteredStream = client.StreamsV2.CreateFilteredStream();

			_filteredStream.TweetReceived += async (sender, tweetV2Event) =>
			{
				var triggerData = new TriggeredFunctionData
				{
					TriggerValue = tweetV2Event
				};
				await Executor.TryExecuteAsync(triggerData, CancellationToken.None);
			};

			await _filteredStream.StartAsync();
		}

		private void HandleFilteredStreamRulesResponseErrors(FilteredStreamRulesV2Response response)
		{
			if(response.Errors != null && response.Errors.Count() > 0)
			{
				var error = response.Errors.First();
				throw new Exception($"{error.Title} {error.Reason} {error.Detail}");
			}

		}

		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_filteredStream.StopStream();
			return Task.CompletedTask;
		}
	}
}