using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TwitterTriggerExtension;
using Tweetinvi.Events;
using System.Linq;
using Tweetinvi.Events.V2;

namespace TweetMonitor
{
    public static class TweetMonitor
    {
        [FunctionName("TweetMonitor")]
        public static async Task Run([TwitterTrigger(filter: "#azurefunctions")]MatchedTweetReceivedEventArgs tweetEvent, ILogger log)
        {
            log.LogInformation($"\n@{tweetEvent.Tweet.CreatedBy.ScreenName}[{tweetEvent.Tweet.CreatedBy.Name}]\n" +
                $"Tweeted: {tweetEvent.Tweet.FullText}\n" +
                $"HashTags: [{String.Join(",", tweetEvent.Tweet.Hashtags.Select(x => $"#{x.Text}"))}]");
        }

        [FunctionName("TweetMonitorV2Api")]
        public static async Task RunV2([TwitterTriggerV2(user: "MirrorReaderBot")] FilteredStreamTweetV2EventArgs tweetEvent, ILogger log)
        {
            log.LogInformation("\nGot a tweet.");
            log.LogInformation($"TweetId: {tweetEvent.Tweet.Id}\n" +
                $"AuthorId: {tweetEvent.Tweet.AuthorId}\n" +
                $"Tweeted: {tweetEvent.Tweet.Text}\n");
        }
    }
}
