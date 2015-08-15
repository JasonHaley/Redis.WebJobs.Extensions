using System;
using System.Threading;
using System.Threading.Tasks;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class RedisPubSubMessageToStringConverter : IAsyncConverter<string, string>
    {
        public Task<string> ConvertAsync(string input, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
                        
            return Task.FromResult(input);
        }
    }
}

