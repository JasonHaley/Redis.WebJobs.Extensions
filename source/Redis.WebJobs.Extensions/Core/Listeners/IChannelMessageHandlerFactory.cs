
namespace Redis.WebJobs.Extensions.Listeners
{
    public interface IChannelMessageHandlerFactory
    {
        ChannelMessageHandler Create(ChannelMessageHandlerFactoryContext context);
    }
}
