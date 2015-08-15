using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions.Bindings
{
    public class RedisPublishAttributeBindingProvider : IBindingProvider
    {
        private static readonly IPubSubArgumentBindingProvider InnerProvider =
            new CompositeArgumentBindingProvider(new StringArgumentBindingProvider(), new UserTypeArgumentBindingProvider());

        private RedisConfiguration _config;
        public RedisPublishAttributeBindingProvider(RedisConfiguration config)
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
            RedisPublishAttribute attribute = parameter.GetCustomAttribute<RedisPublishAttribute>(inherit: false);

            if (attribute == null)
            {
                return Task.FromResult<IBinding>(null);
            }

            IArgumentBinding<RedisPubSubEntity> argumentBinding = InnerProvider.TryCreate(parameter);

            if (argumentBinding == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind to type '{0}'.", parameter.ParameterType));
            }

            var account = RedisAccount.CreateDbFromConnectionString(_config.ConnectionString);

            IBinding binding = new RedisPublishBinding(parameter.Name, argumentBinding, account,
                attribute.ChannelName);

            return Task.FromResult(binding);
        }
    }
}
