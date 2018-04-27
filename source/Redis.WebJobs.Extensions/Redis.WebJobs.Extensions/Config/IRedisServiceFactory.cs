using Redis.WebJobs.Extensions.Services;

namespace Redis.WebJobs.Extensions
{
    interface IRedisServiceFactory
    {
        IRedisService CreateService(string connectionString);
    }
}
