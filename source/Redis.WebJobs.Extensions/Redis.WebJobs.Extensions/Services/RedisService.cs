
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Services
{
    internal class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        
        public RedisService(string connectionString)
        {
            _connection = ConnectionMultiplexer.Connect(connectionString);
            _database = _connection.GetDatabase();
        }

        public ISubscriber GetSubscriber()
        {
            return _connection.GetSubscriber();
        }

        public async Task SendAsync(string channel, string message)
        {
            await _database.PublishAsync(channel, message);
        }

        public async Task<string> GetAsync(string key)
        {
            RedisValue value = await _database.StringGetAsync(key);
            if (!value.HasValue)
            {
                return null;
            }
            else
            {
                return value.ToString();
            }
        }

        public async Task SetAsync(string key, string value)
        {
            await _database.StringSetAsync(key, value);
        }        

        public async Task<bool> KeyExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }
    }
}
