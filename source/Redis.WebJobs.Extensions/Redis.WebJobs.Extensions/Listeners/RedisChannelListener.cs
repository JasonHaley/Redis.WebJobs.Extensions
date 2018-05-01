using Microsoft.Azure.WebJobs.Host.Executors;
using Redis.WebJobs.Extensions.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisChannelListener : ListenerBase
    {
        private readonly RedisConfiguration _configuration;
        private readonly ITriggeredFunctionExecutor _triggerExecutor;
        private readonly IRedisAttribute _attribute;
        private readonly RedisProcessor _redisProcessor;
        private RedisChannelReceiver _receiver;

        public RedisChannelListener(RedisConfiguration configuration, IRedisAttribute attribute, ITriggeredFunctionExecutor triggerExecutor)
        {
            _configuration = configuration;
            _attribute = attribute;
            _triggerExecutor = triggerExecutor;
            _redisProcessor = CreateProcessor(_attribute.ChannelOrKey);
        }

        protected override void OnStarting()
        {
            if (_receiver != null)
            {
                throw new InvalidOperationException("The listener has already been started.");
            }

            base.OnStarting();
        }

        protected override async Task StartAsyncCore(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _receiver = CreateReceiver(_configuration, _attribute);
            await _receiver.OnMessageAsync(ProcessMessageAsync);
        }

        protected override void OnStopping()
        {
            if (_receiver == null)
            {
                throw new InvalidOperationException(
                    "The listener has not yet been started or has already been stopped.");
            }
            base.OnStopping();
        }

        protected override async Task StopAsyncCore(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _receiver.CloseAsync();
            _receiver = null;
        }

        private async Task ProcessMessageAsync(string message)
        {
            await ProcessMessageAsync(message, CancellationTokenSource.Token);
        }

        internal async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
        {
            if (!await _redisProcessor.BeginMessageArrivedAsync(message, cancellationToken))
            {
                return;
            }

            TriggeredFunctionData input = new TriggeredFunctionData
            {
                TriggerValue = message
            };

            FunctionResult result = await _triggerExecutor.TryExecuteAsync(input, cancellationToken);

            await _redisProcessor.EndMessageArrivedAsync(message, result, cancellationToken);
        }

        protected override void OnDisposing()
        {
            if (_receiver != null)
            {
                _receiver.Abort();
                _receiver = null;
            }
        }
        
        private RedisChannelReceiver CreateReceiver(RedisConfiguration configuration, IRedisAttribute attribute)
        {
            return new RedisChannelReceiver(configuration, attribute);
        }
    }
}
