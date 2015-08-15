using StackExchange.Redis;

namespace Redis.WebJobs.Extensions
{
    internal class RedisAccount
    {
        public IDatabase RedisDb { get; set; }

        public static RedisAccount CreateDbFromConnectionString(string connectionString)
        {
            return new RedisAccount
            {
                RedisDb = RedisClient.CreateDbFromConnectionString(connectionString)
            };
        }
    }
}
