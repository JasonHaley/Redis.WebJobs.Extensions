using System;
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions.Config;

namespace Redis.WebJobs.Extensions
{
    public static class JobHostConfigurationExtensions
    {
        public static void UseRedis(this JobHostConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.UseRedis(new RedisConfiguration());
        }

        public static void UseRedis(this JobHostConfiguration config, TimeSpan checkCacheFrequency)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var redisConfig = new RedisConfiguration {CheckCacheFrequency = checkCacheFrequency};

            config.UseRedis(redisConfig);
        }

        public static void UseRedis(this JobHostConfiguration config, RedisConfiguration redisConfig)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (redisConfig == null)
            {
                throw new ArgumentNullException(nameof(redisConfig));
            }

            config.RegisterExtensionConfigProvider(new RedisExtensionConfig(redisConfig));
        }
    }
}
