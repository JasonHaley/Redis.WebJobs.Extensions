using Microsoft.Azure.WebJobs.Description;
using System;

namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class RedisAttribute : Attribute, IRedisAttribute
    {
        public RedisAttribute(string channelName)
        {
            ChannelOrKey = channelName;
            Mode = Mode.PubSub;
        }

        public RedisAttribute(string channelOrKey, Mode mode)
        {
            ChannelOrKey = channelOrKey;
            Mode = mode;
        }

        public RedisAttribute(string channelOrKey, Mode mode, string connectionStringSetting)
        {
            ChannelOrKey = channelOrKey;
            Mode = mode;
            ConnectionStringSetting = connectionStringSetting;
        }

        [AutoResolve]
        public string ChannelOrKey { get; private set; }
        
        public Mode Mode { get; set; }

        [AppSetting]
        public string ConnectionStringSetting { get; set; }
    }

    public enum Mode
    {
        PubSub,
        Cache
    }
}
