using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisEntity
    {
        public RedisAccount Account { get; set; }
        public string ChannelOrKey { get; set; }
        public Mode Mode { get; set; }
        public Task SendAsync(string message)
        {
            return Account.RedisDb.PublishAsync(ChannelOrKey, message);
        }
        public async Task SetAsync(string value)
        {
            await Account.RedisDb.StringSetAsync(ChannelOrKey, value, null, When.Always, CommandFlags.None);
        }

        public async Task<string> GetAsync()
        {
            RedisValue value = await Account.RedisDb.StringGetAsync(ChannelOrKey, CommandFlags.None);
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
