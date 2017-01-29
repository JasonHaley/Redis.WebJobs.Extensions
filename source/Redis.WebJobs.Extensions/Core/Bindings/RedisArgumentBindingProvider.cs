using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisArgumentBindingProvider
    {
        public IArgumentBinding<RedisEntity> TryCreate(ParameterInfo parameter)
        {
            Type itemType;
            if (parameter.IsOut)
            {
                itemType = parameter.ParameterType.GetElementType();
            }
            else
            {
                itemType = parameter.ParameterType;
            }

            if (typeof(object) == itemType)
            {
                throw new InvalidOperationException("Object element types are not supported.");
            }

            return CreateBinding(itemType);
        }

        private static IArgumentBinding<RedisEntity> CreateBinding(Type itemType)
        {
            Type genericType = typeof(RedisArgumentBinding<>).MakeGenericType(itemType);
            return (IArgumentBinding<RedisEntity>)Activator.CreateInstance(genericType);
        }

        private class RedisArgumentBinding<TInput> : IArgumentBinding<RedisEntity>
        {
            public Type ValueType => typeof (TInput);

            public Task<IValueProvider> BindAsync(RedisEntity value, ValueBindingContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                IValueProvider provider = new RedisValueBinder<TInput>(value);

                return Task.FromResult(provider);
            }
        }
    }
}
