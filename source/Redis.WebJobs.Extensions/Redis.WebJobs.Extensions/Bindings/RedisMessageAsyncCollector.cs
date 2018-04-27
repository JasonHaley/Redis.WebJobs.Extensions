using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions.Services;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisMessageAsyncCollector : IAsyncCollector<string>
    {
        private readonly List<string> _messages = new List<string>();
        private readonly RedisConfiguration _configuration;
        private readonly RedisAttribute _attribute;
        private readonly IRedisService _service;

        public RedisMessageAsyncCollector(RedisConfiguration configuration, RedisAttribute attribute, IRedisService service)
        {
            _configuration = configuration;
            _attribute = attribute;
            _service = service;
        }

        public Task AddAsync(string item, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(item))
            {
                throw new ArgumentNullException("item");
            }

            _messages.Add(item);

            return Task.CompletedTask;
        }

        public string Convert(RedisAttribute input)
        {
            return "";
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach(var message in _messages)
            {
                await _service.SendAsync(_attribute.ChannelOrKey, message);
            }
        }
    }
}
