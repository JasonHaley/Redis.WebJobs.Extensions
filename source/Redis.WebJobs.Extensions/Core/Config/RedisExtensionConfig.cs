using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Redis.WebJobs.Extensions.Bindings;
using Redis.WebJobs.Extensions.Triggers;

namespace Redis.WebJobs.Extensions.Config
{
    internal class RedisExtensionConfig : IExtensionConfigProvider
    {
        private readonly RedisConfiguration _redisConfig;

        public RedisExtensionConfig(RedisConfiguration redisConfig)
        {
            _redisConfig = redisConfig ?? throw new ArgumentNullException(nameof(redisConfig));
        }

        public RedisConfiguration Config => _redisConfig;

        /// <inheritdoc />
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Config.RegisterBindingExtensions(
                new RedisTriggerAttributeBindingProvider(_redisConfig, context.Trace),
                new RedisAttributeBindingProvider(_redisConfig, context.Trace));
        }
    }
}
