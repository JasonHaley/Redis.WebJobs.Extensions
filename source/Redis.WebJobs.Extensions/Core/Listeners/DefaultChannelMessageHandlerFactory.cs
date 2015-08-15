using System;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class DefaultChannelMessageHandlerFactory : IChannelMessageHandlerFactory
    {
        public ChannelMessageHandler Create(ChannelMessageHandlerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return new ChannelMessageHandler(context);
        }
    }
}
