using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisKeyEntity
    {
        public RedisAccount Account { get; set; }
        public string KeyName { get; set; }
        public async Task AddOrReplaceAsync(string value)
        {
            await Account.RedisDb.StringSetAsync(KeyName, value, null, When.Always, CommandFlags.None);
        }

        public async Task<string> GetAsync()
        {
            RedisValue value = await Account.RedisDb.StringGetAsync(KeyName, CommandFlags.None);
            if (!value.HasValue)
            {
                return null;
            }
            else
            {
                return (string)value;
            }
        }
    }
}
