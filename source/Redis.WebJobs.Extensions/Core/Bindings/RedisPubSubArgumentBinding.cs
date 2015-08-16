using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.WindowsAzure.Storage.Table;
using Redis.WebJobs.Extensions.Framework;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisPubSubArgumentBindingProvider
    {
        public IArgumentBinding<RedisPubSubEntity> TryCreate(ParameterInfo parameter)
        {
            if (!parameter.IsOut)
            {
                return null;
            }

            Type itemType = parameter.ParameterType.GetElementType();

            if (typeof(object) == itemType)
            {
                throw new InvalidOperationException("Object element types are not supported.");
            }

            return CreateBinding(itemType);
        }

        private static IArgumentBinding<RedisPubSubEntity> CreateBinding(Type itemType)
        {
            Type genericType = typeof(RedisPubSubArgumentBinding<>).MakeGenericType(itemType);
            return (IArgumentBinding<RedisPubSubEntity>)Activator.CreateInstance(genericType);
        }

        private class RedisPubSubArgumentBinding<TInput> : IArgumentBinding<RedisPubSubEntity>
        {
            public Type ValueType
            {
                get { return typeof (TInput); }
            }

            public Task<IValueProvider> BindAsync(RedisPubSubEntity value, ValueBindingContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                IValueProvider provider = new RedisPubSubValueBinder<TInput>(value);

                return Task.FromResult(provider);
            }
        }
    }
}
