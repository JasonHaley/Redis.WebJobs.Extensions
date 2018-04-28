using Redis.WebJobs.Extensions.Services;

namespace Redis.WebJobs.Extensions
{
    internal interface IRedisServiceFactory
    {
        IRedisService CreateService(string connectionString);
    }
}
