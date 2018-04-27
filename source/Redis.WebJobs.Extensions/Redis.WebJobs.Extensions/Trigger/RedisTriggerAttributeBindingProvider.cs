using Microsoft.Azure.WebJobs.Host.Triggers;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Trigger
{
    internal class RedisTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly RedisConfiguration _configuration;
        public RedisTriggerAttributeBindingProvider(RedisConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ParameterInfo parameter = context.Parameter;
            RedisTriggerAttribute attribute = parameter.GetCustomAttribute<RedisTriggerAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            ITriggerBinding binding = new RedisTriggerBinding(_configuration, parameter, attribute);

            return Task.FromResult(binding);
        }
    }
}
