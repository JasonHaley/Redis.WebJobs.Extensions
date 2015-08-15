using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisPubSubTriggerExecutor
    {
        private readonly ITriggeredFunctionExecutor _innerExecutor;

        public RedisPubSubTriggerExecutor(ITriggeredFunctionExecutor innerExecutor)
        {
            _innerExecutor = innerExecutor;
        }

        public async Task<FunctionResult> ExecuteAsync(string value, CancellationToken cancellationToken)
        {
            TriggeredFunctionData input = new TriggeredFunctionData
            {
                TriggerValue = value
            };
            return await _innerExecutor.TryExecuteAsync(input, cancellationToken);
        }
    }
}
