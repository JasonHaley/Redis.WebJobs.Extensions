
using Microsoft.Azure.WebJobs;
using System;

namespace Redis.WebJobs.Extensions
{
    public static class RedisJobHostConfigurationExtensions
    {
        public static void UseRedis(this JobHostConfiguration config, RedisConfiguration redisConfig = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (redisConfig == null)
            {
                redisConfig = new RedisConfiguration();
            }

            config.RegisterExtensionConfigProvider(redisConfig);
        }

        public static void UseRedis(this JobHostConfiguration config, TimeSpan checkCacheFrequency)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            var redisConfig = new RedisConfiguration();
            redisConfig.CheckCacheFrequency = checkCacheFrequency;

            config.UseRedis(redisConfig);
        }
        
    }
}
