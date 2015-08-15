using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisChannelListenerFactory : IListenerFactory
    {
        private readonly RedisAccount _account;
        private readonly string _channelName;
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly RedisConfiguration _config;
        
        public RedisChannelListenerFactory(RedisAccount account, string channelName, ITriggeredFunctionExecutor executor, RedisConfiguration config)
        {
            _account = account;
            _channelName = channelName;
            _executor = executor;
            _config = config;
        }

        public Task<IListener> CreateAsync(CancellationToken cancellationToken)
        {
            RedisPubSubTriggerExecutor triggerExecutor = new RedisPubSubTriggerExecutor(_executor);
            IListener listener = new RedisChannelListener(_account, _channelName, triggerExecutor, _config);
            return Task.FromResult(listener);
        }
    }
}
