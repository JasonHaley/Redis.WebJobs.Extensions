using Microsoft.Azure.WebJobs.Host.Executors;
using Redis.WebJobs.Extensions.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisCacheListener : ListenerBase
    {
        private readonly RedisConfiguration _configuration;
        private readonly ITriggeredFunctionExecutor _triggerExecutor;
        private readonly IRedisAttribute _attribute;
        private readonly RedisProcessor _redisProcessor;
        private readonly string _lastValueKeyName;
        private Timer _timer;
        private TimeSpan _remainingInterval;

        public RedisCacheListener(RedisConfiguration configuration, IRedisAttribute attribute, ITriggeredFunctionExecutor triggerExecutor)
        {
            _configuration = configuration;
            _attribute = attribute;
            _triggerExecutor = triggerExecutor;
            _redisProcessor = CreateProcessor(_attribute.ChannelOrKey);
            _lastValueKeyName = _configuration.LastValueKeyNamePrefix + _attribute.ChannelOrKey;
        }

        protected override void OnStarting()
        {
            ThrowIfDisposed();

            if (!_configuration.CheckCacheFrequency.HasValue)
            {
                throw new InvalidOperationException("The lister needs the CheckFrequency set.");
            }

            if (_timer != null && _timer.Enabled)
            {
                throw new InvalidOperationException("The listener has already been started.");
            }

            _timer = new Timer
            {
                AutoReset = false,
                Interval = _configuration.CheckCacheFrequency.Value.TotalMilliseconds,
            };

            _timer.Elapsed += OnTimer;

            base.OnStarting();
        }

        protected override Task StartAsyncCore(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            _timer.Start();

            return Task.FromResult(true);
        }

        protected override void OnStopping()
        {
            ThrowIfDisposed();

            if (_timer == null)
            {
                throw new InvalidOperationException("The listener has not yet been started or has already been stopped.");
            }

            base.OnStopping();
        }

        protected override Task StopAsyncCore(CancellationToken cancellationToken)
        {
            CancellationTokenSource.Cancel();

            _timer.Dispose();
            _timer = null;

            return Task.FromResult(true);
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            HandleTimerEvent().Wait();
        }

        internal async Task HandleTimerEvent()
        {
            if (_remainingInterval != TimeSpan.Zero)
            {
                // if we're in the middle of a long interval that exceeds
                // Timer's max interval, continue the remaining interval w/o
                // invoking the function
                StartTimer(_remainingInterval);
                return;
            }

            DateTime lastOccurrence = DateTime.Now;

            var receiver = CreateReceiver(_configuration, _attribute, _lastValueKeyName);
            await receiver.OnExecuteAsync(ProcessMessageAsync);

            // restart the timer with the next schedule occurrence
            if (_configuration.CheckCacheFrequency != null)
            {
                DateTime nextOccurrence = DateTime.Now + _configuration.CheckCacheFrequency.Value;
                TimeSpan nextInterval = nextOccurrence - lastOccurrence;
                StartTimer(nextInterval);
            }
        }

        internal void StartTimer(TimeSpan interval)
        {
            if (_configuration.CheckCacheFrequency != null && interval > _configuration.CheckCacheFrequency.Value)
            {
                // if the interval exceeds the maximum interval supported by Timer,
                // store the remainder and use the max
                _remainingInterval = interval - _configuration.CheckCacheFrequency.Value;
                interval = _configuration.CheckCacheFrequency.Value;
            }
            else
            {
                // clear out any remaining interval
                _remainingInterval = TimeSpan.Zero;
            }

            _timer.Interval = interval.TotalMilliseconds;
            _timer.Start();
        }

        private Task ProcessMessageAsync(string previousValue, string currentValue)
        {
            return ProcessMessageAsync(previousValue, currentValue, CancellationTokenSource.Token);
        }

        internal async Task ProcessMessageAsync(string previousValue, string currentValue, CancellationToken cancellationToken)
        {
            if (!await _redisProcessor.BeginMessageArrivedAsync(currentValue, cancellationToken))
            {
                return;
            }

            TriggeredFunctionData input = new TriggeredFunctionData
            {
                TriggerValue = currentValue
            };

            FunctionResult result = await _triggerExecutor.TryExecuteAsync(input, cancellationToken);

            await _redisProcessor.EndMessageArrivedAsync(currentValue, result, cancellationToken);
        }

        protected override void OnDisposing()
        {
            if (!Disposed)
            {
                // Running callers might still be using the cancellation token.
                // Mark it canceled but don't dispose of the source while the callers are running.
                // Otherwise, callers would receive ObjectDisposedException when calling token.Register.
                // For now, rely on finalization to clean up _cancellationTokenSource's wait handle (if allocated).
                CancellationTokenSource.Cancel();

                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                Disposed = true;
            }
        }

        private RedisCacheReceiver CreateReceiver(RedisConfiguration configuration, IRedisAttribute attribute, string lastValueKeyName)
        {
            return new RedisCacheReceiver(configuration, attribute, lastValueKeyName);
        }

    }
}
