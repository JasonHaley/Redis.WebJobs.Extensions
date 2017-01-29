using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;

namespace Redis.WebJobs.Extensions.Framework
{
    internal class JsonValueProvider : IValueProvider
    {
        private readonly object _value;
        private readonly Type _valueType;
        private readonly string _invokeString;
        
        public JsonValueProvider(object value, Type valueType)
        {
            string message = value as string;
            if (message == null)
            {
                throw new InvalidOperationException("Unable to convert message.");
            }
            
            _valueType = valueType;
            _invokeString = message;
            
            _value = CreateValue(value);
        }

        private object CreateValue(object value)
        {
            var contents = TryJsonConvert(_invokeString);
            if (contents != null)
            {
                return contents;
            }
            else
            {
                // 2. if its a string, just use it then
                if (_valueType == typeof(string))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
        }

        public Type Type => _valueType;

        public object GetValue()
        {
            return _value;
        }
        public Task<object> GetValueAsync()
        {
            return Task.FromResult(GetValue());
        }

        public string ToInvokeString()
        {
            return _invokeString;
        }

        private object TryJsonConvert(string message)
        {
            try
            {
                return JsonConvert.DeserializeObject(message, _valueType, Constants.JsonSerializerSettings);
            }
            catch (JsonException ex)
            {
                return null;
            }
        }
    }
}
