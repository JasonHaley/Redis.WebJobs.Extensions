using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Newtonsoft.Json;
using Redis.WebJobs.Extensions.Bindings;

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

                try
                {
                    TInput contents = await GetBody(value, context);

                    if (contents == null)
                    {
                        provider = await RedisPubSubMessageValueProvider.CreateAsync(value, null, ValueType,
                            context.CancellationToken);
                        return new TriggerData(provider, null);
                    }

                    provider = await RedisPubSubMessageValueProvider.CreateAsync(value, contents, ValueType,
                        context.CancellationToken);

                    IReadOnlyDictionary<string, object> bindingData = (_bindingDataProvider != null)
                        ? _bindingDataProvider.GetBindingData(contents) : null;

                    return new TriggerData(provider, bindingData);
                }
                catch (Exception)
                {
                    if (typeof(TInput) == typeof(string))
                    {
                        provider = await RedisPubSubMessageValueProvider.CreateAsync(value, value, ValueType,
                            context.CancellationToken);

                        return new TriggerData(provider, null);
                    }
                    else
                    {
                        provider = await RedisPubSubMessageValueProvider.CreateAsync(value, null, ValueType,
                            context.CancellationToken);
                        return new TriggerData(provider, null);
                    }
                }

            }

            private static Task<TInput> GetBody(string message, ValueBindingContext context)
            {
                try
                {
                    return Task.FromResult(JsonConvert.DeserializeObject<TInput>(message, Constants.JsonSerializerSettings));
                }
                catch (JsonException e)
                {

                    // Easy to have the queue payload not deserialize properly. So give a useful error. 
                    string msg = string.Format(
    @"Binding parameters to complex objects (such as '{0}') uses Json.NET serialization. 
1. Bind the parameter type as 'string' instead of '{0}' to get the raw values and avoid JSON deserialization, or
2. Change the queue payload to be valid json. The JSON parser failed: {1}
", typeof(TInput).Name, e.Message);
                    throw new InvalidOperationException(msg);

                }
            }
        }
    }
}

