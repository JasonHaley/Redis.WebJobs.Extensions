using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisGetArgumentBindingProvider
    {
        public IArgumentBinding<RedisKeyEntity> TryCreate(ParameterInfo parameter)
        {
            //if (!parameter.IsOut)
            //{
            //    return null;
            //}

            Type itemType = parameter.ParameterType;

            if (typeof(object) == itemType)
            {
                throw new InvalidOperationException("Object element types are not supported.");
            }

            return CreateBinding(itemType);
        }

        private static IArgumentBinding<RedisKeyEntity> CreateBinding(Type itemType)
        {
            Type genericType = typeof(RedisGetArgumentBinding<>).MakeGenericType(itemType);
            return (IArgumentBinding<RedisKeyEntity>)Activator.CreateInstance(genericType);
        }

        private class RedisGetArgumentBinding<TInput> : IArgumentBinding<RedisKeyEntity>
        {
            public Type ValueType
            {
                get { return typeof(TInput); }
            }

            public Task<IValueProvider> BindAsync(RedisKeyEntity value, ValueBindingContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                IValueProvider provider = new RedisGetValueBinder<TInput>(value);

                return Task.FromResult(provider);
            }
        }
    }
}
