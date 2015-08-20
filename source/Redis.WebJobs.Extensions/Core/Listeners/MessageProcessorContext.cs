using System;

namespace Redis.WebJobs.Extensions.Listeners
{
    public class MessageProcessorContext
    {
        public MessageProcessorContext(string channelOrKey)
        {
            if (string.IsNullOrEmpty(channelOrKey))
            {
                throw new ArgumentNullException("channelOrKey");
            }

            ChannelOrKey = channelOrKey;
        }

        public string ChannelOrKey { get; private set; }
    }
}
