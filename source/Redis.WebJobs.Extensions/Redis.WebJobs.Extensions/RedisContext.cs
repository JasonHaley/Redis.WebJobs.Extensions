using Redis.WebJobs.Extensions.Services;

namespace Redis.WebJobs.Extensions
{
    internal class RedisContext
    {
        public IRedisAttribute ResolvedAttribute { get; set; }
        public IRedisService Service { get; set; }
    }
}
