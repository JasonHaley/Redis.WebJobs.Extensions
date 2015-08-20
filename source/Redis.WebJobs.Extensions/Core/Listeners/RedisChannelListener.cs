using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisChannelListener : IListener
    {
        private readonly RedisAccount _account;
        private readonly RedisConfiguration _config;
        private readonly ITriggeredFunctionExecutor _triggerExecutor;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly MessageProcessor _messageProcessor;
        private readonly string _channelOrKey;
        private readonly Mode _mode;

        private MessageReceiver _receiver;
        private bool _disposed;

        public RedisChannelListener(RedisAccount account, string channelOrKey, Mode mode, ITriggeredFunctionExecutor triggerExecutor, RedisConfiguration config)
        {
            _account = account;
            _channelOrKey = channelOrKey;
            _mode = mode;
            _triggerExecutor = triggerExecutor;
            _cancellationTokenSource = new CancellationTokenSource();
            _config = config;

            _messageProcessor = CreateMessageProcessor(channelOrKey);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (_receiver != null)
            {
                throw new InvalidOperationException("The listener has already been started.");
            }

            return StartAsyncCore(cancellationToken);
        }

        private async Task StartAsyncCore(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _receiver = new MessageReceiver(_config, _channelOrKey);
            await _receiver.OnMessageAsync(ProcessMessageAsync);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (_receiver == null)
            {
                throw new InvalidOperationException(
                    "The listener has not yet been started or has already been stopped.");
            }
            _cancellationTokenSource.Cancel();

            return StopAsyncCore(cancellationToken);
        }

        private async Task StopAsyncCore(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _receiver.CloseAsync();
            _receiver = null;
        }

        public void Cancel()
        {
            ThrowIfDisposed();
            _cancellationTokenSource.Cancel();
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_cancellationTokenSource")]
        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource.Cancel();

                if (_receiver != null)
                {
                    _receiver.Abort();
                    _receiver = null;
                }

                _disposed = true;
            }
        }

        private Task ProcessMessageAsync(string message)
        {
            return ProcessMessageAsync(message, _cancellationTokenSource.Token);
        }

        internal async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
        {
            if (!await _messageProcessor.BeginMessageArrivedAsync(message, cancellationToken))
            {
                return;
            }

            TriggeredFunctionData input = new TriggeredFunctionData
            {
                TriggerValue = message
            };

            FunctionResult result = await _triggerExecutor.TryExecuteAsync(input, cancellationToken);

            await _messageProcessor.EndMessageArrivedAsync(message, result, cancellationToken);
        }

        private MessageProcessor CreateMessageProcessor(string channelName)
        {
            var context = new MessageProcessorContext(channelName);
            return new MessageProcessor(context);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(null);
            }
        }
    }
}
