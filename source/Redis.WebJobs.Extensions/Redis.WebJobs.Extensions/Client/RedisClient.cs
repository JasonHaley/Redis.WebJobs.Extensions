using StackExchange.Redis;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Client
{
    internal class RedisClient
    {
        public async Task<IDatabase> CreateDbFromConnectionStringAsync(string connectionString)
        {
            var redisConnection = await CreateConnectionFromConnectionStringAsync(connectionString);
            return redisConnection.GetDatabase();
        }

        public async Task<IConnectionMultiplexer> CreateConnectionFromConnectionStringAsync(string connectionString)
        {
            return await InnerConnectAsync(connectionString);
        }

        protected async Task<IConnectionMultiplexer> InnerConnectAsync(string connectionString)
        {
            return await ConnectionMultiplexer.ConnectAsync(connectionString);
        }
    }
}
