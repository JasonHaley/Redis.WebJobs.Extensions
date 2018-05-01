using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
        
            JobHostConfiguration config = new JobHostConfiguration();

            config.NameResolver = new NameResolver();
            
            config.UseRedis();

            config.LoggerFactory.AddConsole();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            JobHost host = new JobHost(config);
            host.Start();

            // Give subscriber chance to startup
            Task.Delay(5000).Wait();

            host.Call(typeof(Functions).GetMethod("SendStringMessage"));
            Task.Delay(5000).Wait();

            host.Call(typeof(Functions).GetMethod("SendMultipleStringPubSubMessages"));
            Task.Delay(5000).Wait();

            host.Call(typeof(Functions).GetMethod("SendPocoMessage"));
            Task.Delay(5000).Wait();

            host.Call(typeof(Functions).GetMethod("SetStringToCache"));
            Task.Delay(5000).Wait();

            host.Call(typeof(Functions).GetMethod("SetStringToCacheUsingResolver"));
            Task.Delay(5000).Wait();

            host.Call(typeof(Functions).GetMethod("SetPocoToCache"));
            Task.Delay(5000).Wait();

            Console.CancelKeyPress += (sender, e) =>
            {
                host.Stop();
            };
            
            //host.RunAndBlock();

        }
    }
}
