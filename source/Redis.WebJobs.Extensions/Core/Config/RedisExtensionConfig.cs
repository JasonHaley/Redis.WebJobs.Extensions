using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Redis.WebJobs.Extensions.Bindings;
using Redis.WebJobs.Extensions.Triggers;

namespace Redis.WebJobs.Extensions.Config
{
    internal class RedisExtensionConfig : IExtensionConfigProvider
    {
        private readonly RedisConfiguration _redisConfig;

        public RedisExtensionConfig(RedisConfiguration redisConfig)
        {
            if (redisConfig == null)
            {
                throw new ArgumentNullException("redisConfig");
            }

            _redisConfig = redisConfig;
        }

        public RedisConfiguration Config { get { return _redisConfig; } }

        /// <inheritdoc />
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Config.RegisterBindingExtensions(
                new RedisSubscribeTriggerAttributeBindingProvider(_redisConfig),
                new RedisPublishAttributeBindingProvider(_redisConfig),
                new RedisAddOrReplaceAttributeBindingProvider(_redisConfig),
                new RedisGetAttributeBindingProvider(_redisConfig));
        }
    }
}
