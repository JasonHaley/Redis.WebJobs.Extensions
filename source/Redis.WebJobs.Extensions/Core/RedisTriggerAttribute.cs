using System;


namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RedisTriggerAttribute : Attribute
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

        public string ChannelOrKey { get; private set; }

        public Mode Mode { get; set; }
    }
}
