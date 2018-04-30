using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;
using Redis.WebJobs.Extensions.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisCacheValueBinder<TInput> : IValueBinder
        where TInput : class
    {
        private RedisContext _context;
        private string _originalValue;

        public RedisCacheValueBinder(RedisContext context)
        {
            _context = context;
        }

        public Type Type
        {
            get { return typeof(TInput); }
        }

        public async Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            if (value == null || _originalValue == null)
            {
                return;
            }

            await SetValueInternalAsync(_originalValue, value as TInput, _context);
        }

        public async Task<object> GetValueAsync()
        {
            var value = await _context.Service.GetAsync(_context.ResolvedAttribute.ChannelOrKey);

            if (value == null)
            {
                return default(TInput);
            }

            _originalValue = value;

            TInput contents;
            if (TryJsonConvert(value, out contents))
            {
                return contents;
            }
            else
            {
                return value;
            }
        }

        public string ToInvokeString()
        {
            return _context.ResolvedAttribute.ChannelOrKey;
        }

        internal async Task SetValueInternalAsync(string originalValue, TInput newValue, RedisContext context)
        {
            if (typeof(TInput) == typeof(string))
            {
                return;
            }
            
            var currentValue = ConvertToJson((TInput)newValue);
            
            if (string.Compare(originalValue, currentValue) != 0)
            {
                await _context.Service.SetAsync(_context.ResolvedAttribute.ChannelOrKey, currentValue);
            }
        }

        internal static string ConvertToJson(TInput input)
        {
            return JsonConvert.SerializeObject(input, Constants.JsonSerializerSettings);
        }

        internal static bool TryJsonConvert(string message, out TInput contents)
        {
            contents = default(TInput);
            try
            {
                contents = JsonConvert.DeserializeObject<TInput>(message, Constants.JsonSerializerSettings);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}