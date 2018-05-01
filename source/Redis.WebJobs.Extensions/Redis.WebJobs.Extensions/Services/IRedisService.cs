
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Services
{
    public interface IRedisService
    {
        ISubscriber GetSubscriber();
        Task SendAsync(string channel, string message);
        Task<string> GetAsync(string key);
        Task SetAsync(string key, string value);
        Task<bool> KeyExistsAsync(string key);
    }
}
