using System;
using System.Globalization;
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
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (trace == null)
            {
                throw new ArgumentNullException("trace");
            }
            _config = config;
            _trace = trace;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
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
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind to type '{0}'.", parameter.ParameterType));
            }

            var account = RedisAccount.CreateDbFromConnectionString(_config.ConnectionString);

            IBinding binding = new RedisBinding(parameter.Name, argumentBinding, account, attribute, context, _trace);

            return Task.FromResult(binding);
        }
    }
}
