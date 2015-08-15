using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class UserTypeArgumentBindingProvider : IPubSubArgumentBindingProvider
    {
        public IArgumentBinding<RedisPubSubEntity> TryCreate(ParameterInfo parameter)
        {
            if (!parameter.IsOut)
            {
                return null;
            }

            Type itemType = parameter.ParameterType.GetElementType();

            if (typeof(IEnumerable).IsAssignableFrom(itemType))
            {
                throw new InvalidOperationException("Enumerable types are not supported. Use ICollector<T> or IAsyncCollector<T> instead.");
            }
            else if (typeof(object) == itemType)
            {
                throw new InvalidOperationException("Object element types are not supported.");
            }

            return CreateBinding(itemType);
        }

        private static IArgumentBinding<RedisPubSubEntity> CreateBinding(Type itemType)
        {
            Type genericType = typeof(UserTypeArgumentBinding<>).MakeGenericType(itemType);
            return (IArgumentBinding<RedisPubSubEntity>)Activator.CreateInstance(genericType);
        }

        private class UserTypeArgumentBinding<TInput> : IArgumentBinding<RedisPubSubEntity>
        {
            public Type ValueType
            {
                get { return typeof(TInput); }
            }

            public Task<IValueProvider> BindAsync(RedisPubSubEntity value, ValueBindingContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                IConverter<TInput, string> converter = new UserTypeToStringConverter<TInput>();
                IValueProvider provider = new ConverterValueBinder<TInput>(value, converter);

                return Task.FromResult(provider);
            }
        }
    }
}
