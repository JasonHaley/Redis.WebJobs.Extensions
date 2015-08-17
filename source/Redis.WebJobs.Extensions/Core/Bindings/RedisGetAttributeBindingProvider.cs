using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions.Bindings
{
    public class RedisGetAttributeBindingProvider : IBindingProvider
    {
        private static readonly RedisGetArgumentBindingProvider _provider = new RedisGetArgumentBindingProvider();

        private RedisConfiguration _config;
        public RedisGetAttributeBindingProvider(RedisConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            _config = config;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParameterInfo parameter = context.Parameter;
            RedisGetAttribute attribute = parameter.GetCustomAttribute<RedisGetAttribute>(inherit: false);

            if (attribute == null)
            {
                return Task.FromResult<IBinding>(null);
            }

            IArgumentBinding<RedisKeyEntity> argumentBinding = _provider.TryCreate(parameter);

            if (argumentBinding == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind to type '{0}'.", parameter.ParameterType));
            }

            var account = RedisAccount.CreateDbFromConnectionString(_config.ConnectionString);

            IBinding binding = new RedisGetBinding(parameter.Name, argumentBinding, account, attribute.KeyName);

            return Task.FromResult(binding);
        }
    }
}