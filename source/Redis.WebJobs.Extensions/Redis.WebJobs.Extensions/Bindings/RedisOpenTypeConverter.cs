
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions.Services;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisOpenTypeConverter<T> : IConverter<IRedisAttribute, IAsyncCollector<T>>
    {
        private readonly RedisConfiguration _configuration;

        public RedisOpenTypeConverter(RedisConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IAsyncCollector<T> Convert(IRedisAttribute attribute)
        {
            IRedisService service = _configuration.CreateService(attribute);
            return new RedisMessageAsyncCollector<T>(_configuration, attribute, service);
        }
    }
}
