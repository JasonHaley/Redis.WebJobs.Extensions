using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions.Config;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class PubSubReceiver 
    {
        private readonly RedisConfiguration _config;
        private readonly RedisChannel _channel;
        private readonly TraceWriter _trace;
        private ISubscriber _subscriber;

        public PubSubReceiver(RedisConfiguration config, string channelName, TraceWriter trace)
        {
            _config = config;
            _channel = new RedisChannel(channelName, RedisChannel.PatternMode.Auto);
            _trace = trace;
        }

        public async Task OnMessageAsync(Func<string, Task> processMessageAsync)
        {
            var connection = RedisClient.CreateConnectionFromConnectionString(_config.ConnectionString);
            _subscriber = connection.GetSubscriber();

            await _subscriber.SubscribeAsync(_channel, async (rc, m) => await processMessageAsync(m), CommandFlags.None);

            _trace.Verbose(string.Format("Subscribed to {0} channel", _channel));
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
