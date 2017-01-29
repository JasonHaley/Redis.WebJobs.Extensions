using System;

namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RedisAttribute : Attribute
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

        public string ChannelOrKey { get; private set; }

        public Mode Mode { get; set; }
    }

    public enum Mode
    {
        PubSub,
        Cache
    }
}
