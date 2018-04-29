using Redis.WebJobs.Extensions.Services;
using System;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal class RedisCacheReceiver
    {
        private readonly RedisConfiguration _configuration;
        private readonly IRedisAttribute _attribute;
        private readonly string _lastValueKeyName;
        private readonly IRedisService _service;

        public RedisCacheReceiver(RedisConfiguration configuration, IRedisAttribute attribute, string lastValueKeyName)
        {
            _configuration = configuration;
            _attribute = attribute;
            _lastValueKeyName = lastValueKeyName;
            _service = _configuration.RedisServiceFactory.CreateService(_configuration.ResolveConnectionString(attribute.ConnectionStringSetting));
        }

        public async Task OnExecuteAsync(Func<string, string, Task> processMessageAsync)
        {
            string prevValue = null;
            string currentValue = null;

            if (await _service.KeyExistsAsync(_lastValueKeyName))
            {
                prevValue = await _service.GetAsync(_lastValueKeyName);
            }

            if (await _service.KeyExistsAsync(_attribute.ChannelOrKey))
            {
                currentValue = await _service.GetAsync(_attribute.ChannelOrKey);
            }

            bool hadValue = !string.IsNullOrEmpty(prevValue);
            bool hasValue = !string.IsNullOrEmpty(currentValue);
            if ((hadValue || hasValue) && currentValue != prevValue)
            {
                // set value for comparison next check
                await _service.SetAsync(_lastValueKeyName, currentValue);
                
                // process the value since it has changed
                await processMessageAsync(prevValue, currentValue);
            }
        }
    }
}
