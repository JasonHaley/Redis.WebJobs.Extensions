using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions.Bindings
{
    public class RedisAttributeBindingProvider : IBindingProvider
    {
        private static readonly RedisArgumentBindingProvider Provider = new RedisArgumentBindingProvider();

        private readonly RedisConfiguration _config;
        private readonly TraceWriter _trace;
        
        public RedisAttributeBindingProvider(RedisConfiguration config, TraceWriter trace)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ParameterInfo parameter = context.Parameter;
            RedisAttribute attribute = parameter.GetCustomAttribute<RedisAttribute>(inherit: false);

            if (attribute == null)
            {
                return Task.FromResult<IBinding>(null);
            }

            IArgumentBinding<RedisEntity> argumentBinding = Provider.TryCreate(parameter);

            if (argumentBinding == null)
            {
                throw new InvalidOperationException($"Can't bind to type '{parameter.ParameterType}'.");
            }

            var account = RedisAccount.CreateDbFromConnectionString(_config.ConnectionString);

            IBinding binding = new RedisBinding(parameter.Name, argumentBinding, account, attribute, context, _trace);

            return Task.FromResult(binding);
        }
    }
}
