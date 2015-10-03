using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Redis.WebJobs.Extensions.Config;
using Redis.WebJobs.Extensions.Framework;
using Timer = System.Timers.Timer;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisCacheListener : ListenerBase
    {
        private readonly RedisConfiguration _config;
        private readonly ITriggeredFunctionExecutor _triggerExecutor;
        private readonly RedisProcessor _redisProcessor;
        private readonly string _channelOrKey;
        private readonly string _lastValueKeyName;
        private Timer _timer;
        private TimeSpan _remainingInterval;
        private readonly TraceWriter _trace;

        public RedisCacheListener(string channelOrKey, ITriggeredFunctionExecutor triggerExecutor,
            RedisConfiguration config, TraceWriter trace)
            : base()
        {
            _channelOrKey = channelOrKey;
            _triggerExecutor = triggerExecutor;
            _config = config;
            _lastValueKeyName = _config.LastValueKeyNamePrefix + channelOrKey;
            _redisProcessor = CreateProcessor(channelOrKey);
            _trace = trace;
        }
        
        protected override void OnStarting()
        {
            ThrowIfDisposed();

            if (!_config.CheckCacheFrequency.HasValue)
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
                Interval = _config.CheckCacheFrequency.Value.TotalMilliseconds,
            };

            _timer.Elapsed += OnTimer;

            base.OnStarting();
        }
        
        protected override Task StartAsyncCore(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            
            _timer.Start();

            return Task.FromResult<bool>(true);
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
            _cancellationTokenSource.Cancel();

            _timer.Dispose();
            _timer = null;

            return Task.FromResult<bool>(true);
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

            var receiver = CreateReceiver(_config, _channelOrKey, _lastValueKeyName);
            await receiver.OnExecuteAsync(ProcessMessageAsync);

            // restart the timer with the next schedule occurrence
            DateTime nextOccurrence = DateTime.Now + _config.CheckCacheFrequency.Value;
            TimeSpan nextInterval = nextOccurrence - lastOccurrence;
            StartTimer(nextInterval);
        }

        internal void StartTimer(TimeSpan interval)
        {
            if (interval > _config.CheckCacheFrequency.Value)
            {
                // if the interval exceeds the maximum interval supported by Timer,
                // store the remainder and use the max
                _remainingInterval = interval - _config.CheckCacheFrequency.Value;
                interval = _config.CheckCacheFrequency.Value;
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
            return ProcessMessageAsync(previousValue, currentValue, _cancellationTokenSource.Token);
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
            if (!_disposed)
            {
                // Running callers might still be using the cancellation token.
                // Mark it canceled but don't dispose of the source while the callers are running.
                // Otherwise, callers would receive ObjectDisposedException when calling token.Register.
                // For now, rely on finalization to clean up _cancellationTokenSource's wait handle (if allocated).
                _cancellationTokenSource.Cancel();

                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                _disposed = true;
            }
        }
        
        private RedisProcessor CreateProcessor(string channelName)
        {
            var context = new RedisProcessorContext(channelName);
            return new RedisProcessor(context, _trace);
        }

        private CacheReceiver CreateReceiver(RedisConfiguration config, string channelOrKey, string lastValueKeyName)
        {
            return new CacheReceiver(config, channelOrKey, lastValueKeyName, _trace);
        }


    }
}
