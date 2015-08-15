using System;
using System.Threading;
using System.Threading.Tasks;
using Redis.WebJobs.Extensions.Bindings;

namespace Redis.WebJobs.Extensions.Converters
{
    internal class StringToRedisPubSubEntityConverter : IAsyncConverter<string, RedisPubSubEntity>
    {
        private readonly RedisAccount _account;
        private readonly string _defaultChannel;

        public StringToRedisPubSubEntityConverter(RedisAccount account, string defaultChannel)
        {
            _account = account;
            _defaultChannel = defaultChannel;
        }

        public Task<RedisPubSubEntity> ConvertAsync(string input, CancellationToken cancellationToken)
        {
            string channelName;

            // For convenience, treat an an empty string as a request for the default value.
            if (String.IsNullOrEmpty(input))
            {
                channelName = _defaultChannel;
            }
            else
            {
                channelName = input;
            }

            cancellationToken.ThrowIfCancellationRequested();
            
            var entity = new RedisPubSubEntity
            {
                Account = _account,
                ChannelName = channelName
            };

            return Task.FromResult(entity);
        }
    }
}

