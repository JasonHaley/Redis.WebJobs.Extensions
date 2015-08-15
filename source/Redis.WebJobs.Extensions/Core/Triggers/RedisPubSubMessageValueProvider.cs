using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class RedisPubSubMessageValueProvider : IValueProvider
    {
        private readonly object _value;
        private readonly Type _valueType;
        private readonly string _invokeString;

        private RedisPubSubMessageValueProvider(object value, Type valueType, string invokeString)
        {
            if (value != null && !valueType.IsAssignableFrom(value.GetType()))
            {
                throw new InvalidOperationException("value is not of the correct type.");
            }

            _value = value;
            _valueType = valueType;
            _invokeString = invokeString;
        }

        public Type Type
        {
            get { return _valueType; }
        }

        public object GetValue()
        {
            return _value;
        }

        public string ToInvokeString()
        {
            return _invokeString;
        }

        public static Task<RedisPubSubMessageValueProvider> CreateAsync(string clone, object value,
            Type valueType, CancellationToken cancellationToken)
        {

            return Task.FromResult(new RedisPubSubMessageValueProvider(value, valueType, clone));
        }
    }
}