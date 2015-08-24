using System;
using System.Threading.Tasks;
using Redis.WebJobs.Extensions.Config;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class PubSubReceiver 
    {
        private RedisConfiguration _config;
        private RedisChannel _channel;
        private ISubscriber _subscriber;

        public PubSubReceiver(RedisConfiguration config, string channelName)
        {
            _config = config;
            _channel = new RedisChannel(channelName, RedisChannel.PatternMode.Auto);
        }

        public async Task OnMessageAsync(Func<string, Task> processMessageAsync)
        {
            var connection = RedisClient.CreateConnectionFromConnectionString(_config.ConnectionString);
            _subscriber = connection.GetSubscriber();

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
