using System;
using System.Threading.Tasks;
using Redis.WebJobs.Extensions.Config;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class MessageReceiver
    {
        private RedisConfiguration _config;
        private RedisChannel _channel;
        private ISubscriber _subscriber;

        public MessageReceiver(RedisConfiguration config, string channelName)
        {
            _config = config;
            _channel = new RedisChannel(channelName, RedisChannel.PatternMode.Auto);
        }

        internal async Task OnMessageAsync(Func<string, Task> processMessageAsync)
        {
            var connection = RedisClient.CreateConnectionFromConnectionString(_config.ConnectionString);
            _subscriber = connection.GetSubscriber();

            await _subscriber.SubscribeAsync(_channel, async (rc, m) => await processMessageAsync(m), CommandFlags.None);
        }

        internal Task CloseAsync()
        {
            return _subscriber.UnsubscribeAllAsync();
        }

        internal void Abort()
        {
            _subscriber.UnsubscribeAll();
        }
    }
}
