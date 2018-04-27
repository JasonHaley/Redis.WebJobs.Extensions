
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Services
{
    public interface IRedisService
    {
        ISubscriber GetSubscriber();

        Task SendAsync(string channel, string message);
        Task<RedisValue> StringGetAsync(RedisKey key);
        Task StringSetAsync(RedisKey key, RedisValue value);

    }
}
