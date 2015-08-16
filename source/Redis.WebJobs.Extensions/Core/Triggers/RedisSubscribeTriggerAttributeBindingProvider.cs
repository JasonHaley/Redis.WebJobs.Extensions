using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions.Triggers
{
    public class RedisSubscribeTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private static readonly RedisSubscribeArgumentBindingProvider _provider = new RedisSubscribeArgumentBindingProvider();
        private readonly RedisConfiguration _config;
        
        public RedisSubscribeTriggerAttributeBindingProvider(RedisConfiguration config)
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
            RedisSubscribeTriggerAttribute attribute = parameter.GetCustomAttribute<RedisSubscribeTriggerAttribute>(inherit: false);

            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }
                                    
            ITriggerDataArgumentBinding<string> argumentBinding = _provider.TryCreate(parameter);

            if (argumentBinding == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Can't bind to type '{0}'.", parameter.ParameterType));
            }

            RedisAccount account = RedisAccount.CreateDbFromConnectionString(_config.ConnectionString);
            ITriggerBinding binding;

            binding = new RedisSubscribeTriggerBinding(parameter.Name, parameter.ParameterType, argumentBinding, account, attribute.ChannelName, _config);
            
            return Task.FromResult(binding);
        }

    }
}
