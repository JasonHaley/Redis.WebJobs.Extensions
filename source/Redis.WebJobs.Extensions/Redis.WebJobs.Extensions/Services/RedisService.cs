
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Services
{
    internal class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;

        public ISubscriber GetSubscriber()
        {
            return _connection.GetSubscriber();
        }

        public RedisService(string connectionString)
        {
            _connection = ConnectionMultiplexer.Connect(connectionString);
            _database = _connection.GetDatabase();
        }

        public async Task SendAsync(string channel, string message)
        {
            await _database.PublishAsync(channel, message);
        }

        public Task<RedisValue> StringGetAsync(RedisKey key)
        {
            return null;
        }

        public Task StringSetAsync(RedisKey key, RedisValue value)
        {
            return null;
        }

        
    }
}
