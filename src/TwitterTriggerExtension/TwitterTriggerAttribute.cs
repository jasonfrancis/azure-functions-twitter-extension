using Microsoft.Azure.WebJobs.Description;
using System;

namespace TwitterTriggerExtension
{
    /// <summary>
    /// Attribute used to mark a job function that should be invoked based on
    /// twitter events.
    /// </summary>
    /// <remarks>
    /// The method parameter type should be <see cref="Tweetinvi.Tweet"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class TwitterTriggerAttribute : Attribute
    {
        public string ConsumerKey { get; set; } = "TwitterConsumerKey";
        public string ConsumerSecret { get; set; } = "TwitterConsumerSecret";
        public string AccessKey { get; set; } = "TwitterAccessKey";
        public string AccessSecret { get; set; } = "TwitterAccessSecret";
        public string Filter { get; private set; }
        public string User { get; private set; }

        public TwitterTriggerAttribute(string filter = null, string user = null)
        {
            Filter = filter;
            User = user;
        }
    }

    /// <summary>
    /// Attribute used to mark a job function that should be invoked based on
    /// twitter V2 API events.
    /// </summary>
    /// <remarks>
    /// The method parameter type should be <see cref="Tweetinvi.Tweet"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class TwitterTriggerV2Attribute : Attribute
    {
        public string V2ConsumerKey { get; set; } = "TwitterV2ConsumerKey";
        public string V2ConsumerSecret { get; set; } = "TwitterV2ConsumerSecret";
        public string V2BearerToken { get; set; } = "TwitterV2BearerToken";
        public string Filter { get; private set; }
        public string User { get; private set; }

        public TwitterTriggerV2Attribute(string filter = null, string user = null)
        {
            Filter = filter;
            User = user;
        }
    }
}
