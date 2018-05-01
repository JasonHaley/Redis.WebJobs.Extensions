using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Redis.WebJobs.Extensions.Framework;
using Redis.WebJobs.Extensions.Services;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisMessageAsyncCollector<TInput> : IAsyncCollector<TInput>
    {
        private readonly List<string> _messages = new List<string>();
        private readonly RedisConfiguration _configuration;
        private readonly IRedisAttribute _attribute;
        private readonly IRedisService _service;

        public RedisMessageAsyncCollector(RedisConfiguration configuration, IRedisAttribute attribute, IRedisService service)
        {
            _configuration = configuration;
            _attribute = attribute;
            _service = service;
        }

        public Task AddAsync(TInput item, CancellationToken cancellationToken = default(CancellationToken))
        {
            string message;
            if (typeof(TInput) == typeof(string))
            {
                message = item.ToString();
            }
            else 
            {
                message = ConvertToJson(item);
            }

            if (!string.IsNullOrEmpty(message))
            {
                _messages.Add(message);
            }

            return Task.CompletedTask;
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach(var message in _messages)
            {
                if (_attribute.Mode == Mode.PubSub)
                {
                    await _service.SendAsync(_attribute.ChannelOrKey, message);
                }
                else 
                {
                    await _service.SetAsync(_attribute.ChannelOrKey, message);
                }
            }
        }

        private string ConvertToJson(TInput input)
        {
            return JsonConvert.SerializeObject(input, Constants.JsonSerializerSettings);
        }
    }
}
