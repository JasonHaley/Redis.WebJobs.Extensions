using Microsoft.Azure.WebJobs.Description;
using System;


namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class RedisTriggerAttribute : Attribute, IRedisAttribute
    {
        public RedisTriggerAttribute(string channelName)
        {
            ChannelOrKey = channelName;
            Mode = Mode.PubSub;
        }

        public RedisTriggerAttribute(string channelOrKey, Mode mode)
        {
            ChannelOrKey = channelOrKey;
            Mode = mode;
        }

        [AutoResolve]
        public string ChannelOrKey { get; private set; }
        
        public Mode Mode { get; set; }

        [AppSetting]
        public string ConnectionStringSetting { get; set; }
    }
}
