using StackExchange.Redis;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Client
{
    internal interface IRedisClient
    {
        Task<IDatabase> CreateDbFromConnectionString(string connectionString);

        Task<IConnectionMultiplexer> CreateConnectionFromConnectionString(string connectionString);
    }
}
