using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;

namespace Redis.WebJobs.Extensions.Listeners
{
    public class RedisProcessor
    {
        private readonly TraceWriter _trace;

        public RedisProcessor(RedisProcessorContext context, TraceWriter trace)
        {
            _trace = trace;

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ChannelOrKey = context.ChannelOrKey;
        }

        public string ChannelOrKey { get; set; }

        public virtual async Task<bool> BeginMessageArrivedAsync(string message, CancellationToken cancellationToken)
        {
            _trace.Verbose($"Message Arrived {message}");

            return await Task.FromResult(true);
        }

        public virtual Task EndMessageArrivedAsync(string message, FunctionResult result, CancellationToken cancellationToken)
        {
            if (!result.Succeeded)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            return Task.FromResult(0);
        }
    }
}
