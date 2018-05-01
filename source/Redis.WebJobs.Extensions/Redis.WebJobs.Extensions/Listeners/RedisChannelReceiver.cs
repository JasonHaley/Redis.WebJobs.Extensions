using Redis.WebJobs.Extensions.Services;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisChannelReceiver
    {
        private readonly RedisConfiguration _configuration;
        private readonly RedisChannel _channel;
        private readonly IRedisService _service;
        private ISubscriber _subscriber;

        public RedisChannelReceiver(RedisConfiguration configuration, IRedisAttribute attribute)
        {
            _configuration = configuration;
            _channel = new RedisChannel(attribute.ChannelOrKey, RedisChannel.PatternMode.Auto);
            _service = _configuration.RedisServiceFactory.CreateService(_configuration.ResolveConnectionString(attribute.ConnectionStringSetting));
        }

        public async Task OnMessageAsync(Func<string, Task> processMessageAsync)
        {
            _subscriber = _service.GetSubscriber();
            await _subscriber.SubscribeAsync(_channel, async (rc, m) => await processMessageAsync(m), CommandFlags.None);
        }

        public Task CloseAsync()
        {
            return _subscriber.UnsubscribeAllAsync();
        }

        public void Abort()
        {
            _subscriber.UnsubscribeAll();
        }
    }
}
