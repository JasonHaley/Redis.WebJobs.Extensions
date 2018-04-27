using Microsoft.Azure.WebJobs.Host.Executors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Listeners
{
    public class RedisProcessor
    {

        public RedisProcessor(RedisProcessorContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ChannelOrKey = context.ChannelOrKey;
        }

        public string ChannelOrKey { get; set; }

        public virtual async Task<bool> BeginMessageArrivedAsync(string message, CancellationToken cancellationToken)
        {   
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
