using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class ConverterArgumentBindingProvider<T> : IPubSubTriggerArgumentBindingProvider
    {
        private readonly IAsyncConverter<string, T> _converter;
        public ConverterArgumentBindingProvider(IAsyncConverter<string, T> converter)
        {
            _converter = converter;
        }

        public ITriggerDataArgumentBinding<string> TryCreate(ParameterInfo parameter)
        {
            if (parameter.ParameterType != typeof(T))
            {
                return null;
            }

            return new ConverterArgumentBinding(_converter);
        }

        internal class ConverterArgumentBinding : ITriggerDataArgumentBinding<string>
        {
            private readonly IAsyncConverter<string, T> _converter;

            public ConverterArgumentBinding(IAsyncConverter<string, T> converter)
            {
                _converter = converter;
            }

            public Type ValueType
            {
                get { return typeof(T); }
            }

            public IReadOnlyDictionary<string, Type> BindingDataContract
            {
                get { return null; }
            }

            public async Task<ITriggerData> BindAsync(string value, ValueBindingContext context)
            {
                object converted = await _converter.ConvertAsync(value, context.CancellationToken);
                IValueProvider provider = await RedisPubSubMessageValueProvider.CreateAsync(value, converted, typeof(T),
                    context.CancellationToken);
                return new TriggerData(provider, null);
            }
        }
    }
}
