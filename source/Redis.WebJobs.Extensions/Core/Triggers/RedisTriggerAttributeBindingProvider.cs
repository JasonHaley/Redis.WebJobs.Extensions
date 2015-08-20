using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions.Triggers
{
    public class RedisTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly RedisConfiguration _config;
        
        public RedisTriggerAttributeBindingProvider(RedisConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            _config = config;
        }
        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParameterInfo parameter = context.Parameter;
            RedisTriggerAttribute attribute = parameter.GetCustomAttribute<RedisTriggerAttribute>(inherit: false);

            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }
            
            RedisAccount account = RedisAccount.CreateDbFromConnectionString(_config.ConnectionString);
            ITriggerBinding binding = new RedisTriggerBinding(parameter, account, attribute.ChannelOrKey, attribute.Mode, _config);
            
            return Task.FromResult(binding);
        }
    }
}
