using System;
using System.Threading.Tasks;
using Redis.WebJobs.Extensions.Config;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class CacheReceiver
    {
        private readonly RedisConfiguration _config;
        private readonly string _channelOrKey;
        private readonly string _lastValueKeyName;
        public CacheReceiver(RedisConfiguration config, string channelOrKey, string lastValueKeyName)
        {
            _config = config;
            _channelOrKey = channelOrKey;
            _lastValueKeyName = lastValueKeyName;
        }

        public async Task OnExecuteAsync(Func<string, string, Task> processMessageAsync)
        {
            var connection = RedisClient.CreateConnectionFromConnectionString(_config.ConnectionString);
            var db = connection.GetDatabase();

            string prevValue = null;
            string currentValue = null;

            if (await db.KeyExistsAsync(_lastValueKeyName))
            {
                prevValue = await db.StringGetAsync(_lastValueKeyName, CommandFlags.None);
            }

            if (await db.KeyExistsAsync(_channelOrKey))
            {
                currentValue = await db.StringGetAsync(_channelOrKey, CommandFlags.None);
            }

            bool hadValue = !string.IsNullOrEmpty(prevValue);
            bool hasValue = !string.IsNullOrEmpty(currentValue);
            if ((hadValue || hasValue) && currentValue != prevValue)
            {
                // set value for comparison next check
                db.StringSet(_lastValueKeyName, currentValue);

                // process the value since it has changed
                await processMessageAsync(prevValue, currentValue);
            }
        }
    }
}
