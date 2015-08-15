using System.Collections.Generic;
using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class CompositeArgumentBindingProvider : IPubSubArgumentBindingProvider
    {
        private readonly IEnumerable<IPubSubArgumentBindingProvider> _providers;

        public CompositeArgumentBindingProvider(params IPubSubArgumentBindingProvider[] providers)
        {
            _providers = providers;
        }

        public IArgumentBinding<RedisPubSubEntity> TryCreate(ParameterInfo parameter)
        {
            foreach (IPubSubArgumentBindingProvider provider in _providers)
            {
                IArgumentBinding<RedisPubSubEntity> binding = provider.TryCreate(parameter);

                if (binding != null)
                {
                    return binding;
                }
            }

            return null;
        }
    }
}
