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
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ChannelOrKey = context.ChannelOrKey;
            _trace = trace;
        }

        public string ChannelOrKey { get; set; }

        public virtual async Task<bool> BeginMessageArrivedAsync(string message, CancellationToken cancellationToken)
        {
            return await Task.FromResult<bool>(true);
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
