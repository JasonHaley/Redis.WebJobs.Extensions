using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Redis.WebJobs.Extensions.Config;
using Redis.WebJobs.Extensions.Framework;
using Redis.WebJobs.Extensions.Listeners;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisChannelListener : ListenerBase
    {
        private readonly RedisConfiguration _config;
        private readonly ITriggeredFunctionExecutor _triggerExecutor;
        private readonly RedisProcessor _redisProcessor;
        private readonly string _channelOrKey;
        private PubSubReceiver _receiver;
        private readonly TraceWriter _trace;

        public RedisChannelListener(string channelOrKey, ITriggeredFunctionExecutor triggerExecutor, RedisConfiguration config, TraceWriter trace)
            : base()
        {
            _channelOrKey = channelOrKey;
            _triggerExecutor = triggerExecutor;
            _config = config;

            _redisProcessor = CreateProcessor(channelOrKey);
            _trace = trace;
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

            _receiver = CreateReceiver(_config, _channelOrKey);
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

        private Task ProcessMessageAsync(string message)
        {
            return ProcessMessageAsync(message, _cancellationTokenSource.Token);
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

        private RedisProcessor CreateProcessor(string channelName)
        {
            var context = new RedisProcessorContext(channelName);
            return new RedisProcessor(context, _trace);
        }

        private PubSubReceiver CreateReceiver(RedisConfiguration config, string channelOrKey)
        {
            return new PubSubReceiver(config, channelOrKey, _trace);
        }
    }
}
