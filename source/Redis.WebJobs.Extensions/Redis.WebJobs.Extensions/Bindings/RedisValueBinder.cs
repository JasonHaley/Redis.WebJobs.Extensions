using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisValueBinder<T> : IValueBinder // AKA IValueProvider
        where T : class
    {
        private RedisContext _context;
        
        public RedisValueBinder(RedisContext context)
        {
            _context = context;
        }

        public Type Type => typeof(T);

        public async Task<object> GetValueAsync()
        {
            RedisValue value = await _context.Service.StringGetAsync(_context.ResolvedAttribute.ChannelOrKey);
            if (!value.HasValue)
            {
                return null;
            }
            else
            {
                return (string)value;
            }
        }

        public async Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            if (value == null)
            {
                return;
            }
            await SetValueInternalAsync(value as T, _context);
        }
        
        public string ToInvokeString()
        {
            return string.Empty;
        }

        internal static async Task SetValueInternalAsync(T value, RedisContext context)
        {
            JObject jValue = JObject.FromObject(value);
            
            if (context.ResolvedAttribute.Mode == Mode.PubSub)
            {

            }
            else
            {
                await context.Service.StringSetAsync(context.ResolvedAttribute.ChannelOrKey, jValue.ToString());
            }
        }
    }
}
