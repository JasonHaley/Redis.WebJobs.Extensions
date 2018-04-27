
using Redis.WebJobs.Extensions.Services;

namespace Redis.WebJobs.Extensions
{
    internal class DefaultRedisServiceFactory : IRedisServiceFactory
    {
        public IRedisService CreateService(string connectionString)
        {
            return new RedisService(connectionString);
        }
    }
}
