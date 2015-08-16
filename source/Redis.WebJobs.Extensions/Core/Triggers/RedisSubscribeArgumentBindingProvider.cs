using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Newtonsoft.Json;
using Redis.WebJobs.Extensions.Framework;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class RedisSubscribeArgumentBindingProvider
    {
        public ITriggerDataArgumentBinding<string> TryCreate(ParameterInfo parameter)
        {
            return CreateBinding(parameter.ParameterType);
        }

        private static ITriggerDataArgumentBinding<string> CreateBinding(Type itemType)
        {
            Type genericType = typeof(RedisSubscibeArgumentBinding<>).MakeGenericType(itemType);
            return (ITriggerDataArgumentBinding<string>)Activator.CreateInstance(genericType);
        }

        private class RedisSubscibeArgumentBinding<TInput> : ITriggerDataArgumentBinding<string>
        {
            private readonly IBindingDataProvider _bindingDataProvider;

            public RedisSubscibeArgumentBinding()
            {
                _bindingDataProvider = BindingDataProvider.FromType(typeof(TInput));
            }

            public Type ValueType
            {
                get { return typeof(TInput); }
            }

            public IReadOnlyDictionary<string, Type> BindingDataContract
            {
                get { return _bindingDataProvider != null ? _bindingDataProvider.Contract : null; }
            }

            public async Task<ITriggerData> BindAsync(string value, ValueBindingContext context)
            {
                IValueProvider provider;

                TInput contents;

                if (TryJsonConvert(value, out contents))
                {
                    provider = await RedisPubSubMessageValueProvider.CreateAsync(value, contents, ValueType,
                        context.CancellationToken);

                    IReadOnlyDictionary<string, object> bindingData = (_bindingDataProvider != null)
                        ? _bindingDataProvider.GetBindingData(contents) : null;

                    return new TriggerData(provider, bindingData);
                }
                else
                {
                    if (typeof(TInput) == typeof(string))
                    {
                        provider = await RedisPubSubMessageValueProvider.CreateAsync(value, value, ValueType,
                            context.CancellationToken);
                    }
                    else
                    {
                        provider = await RedisPubSubMessageValueProvider.CreateAsync(value, null, ValueType,
                            context.CancellationToken);
                    }
                    return new TriggerData(provider, null);
                }

            }

            private bool TryJsonConvert(string message, out TInput contents)
            {
                contents = default(TInput);
                try
                {
                    contents = JsonConvert.DeserializeObject<TInput>(message, Constants.JsonSerializerSettings);
                    return true;
                }
                catch (JsonException e)
                {
                    return false;
                }
            }
        }
    }
}

