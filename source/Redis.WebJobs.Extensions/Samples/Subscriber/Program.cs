using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Redis.WebJobs.Extensions;
using System;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();

            config.NameResolver = new NameResolver();

            var redisConfig = new RedisConfiguration();
            redisConfig.CheckCacheFrequency = TimeSpan.FromSeconds(10);

            config.UseRedis(redisConfig);
            
            config.UseTimers();

            config.LoggerFactory.AddConsole();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
