using System;

namespace Redis.WebJobs.Extensions.Listeners
{
    public class ChannelMessageHandlerFactoryContext
    {
        public ChannelMessageHandlerFactoryContext(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException("channelName");
            }

            ChannelName = channelName;
        }

        public string ChannelName { get; private set; }
    }
}
