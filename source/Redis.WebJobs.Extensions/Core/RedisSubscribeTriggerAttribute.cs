using System;

namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RedisSubscribeTriggerAttribute : Attribute
    {
        public RedisSubscribeTriggerAttribute(string channelName)
        {
            ChannelName = channelName;
        }

        public string ChannelName { get; private set; }
    }
}
