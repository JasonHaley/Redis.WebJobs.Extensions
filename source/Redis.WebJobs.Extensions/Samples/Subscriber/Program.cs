using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Redis.WebJobs.Extensions;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();

            config.NameResolver = new NameResolver();

            config.UseRedis();
            
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
