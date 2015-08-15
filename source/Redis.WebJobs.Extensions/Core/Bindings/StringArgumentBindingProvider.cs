using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class StringArgumentBindingProvider : IPubSubArgumentBindingProvider
    {
        public IArgumentBinding<RedisPubSubEntity> TryCreate(ParameterInfo parameter)
        {
            if (!parameter.IsOut || parameter.ParameterType != typeof(string).MakeByRefType())
            {
                return null;
            }

            return new StringArgumentBinding();
        }
        private class StringArgumentBinding : IArgumentBinding<RedisPubSubEntity>
        {
            public Type ValueType
            {
                get { return typeof(string); }
            }

            
            public Task<IValueProvider> BindAsync(RedisPubSubEntity value, ValueBindingContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                IValueProvider provider = new NonNullConverterValueBinder<string>(value, new StringToRedisPubSubMessageConverter());

                return Task.FromResult(provider);
            }
        }
    }
}
