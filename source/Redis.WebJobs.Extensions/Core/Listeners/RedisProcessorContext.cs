using System;

namespace Redis.WebJobs.Extensions.Listeners
{
    public class RedisProcessorContext
    {
        public RedisProcessorContext(string channelOrKey)
        {
            if (string.IsNullOrEmpty(channelOrKey))
            {
                throw new ArgumentNullException(nameof(channelOrKey));
            }

            ChannelOrKey = channelOrKey;
        }

        public string ChannelOrKey { get; private set; }
    }
}
