using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;
using Redis.WebJobs.Extensions.Framework;

namespace SamplePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();
            //config.Tracing.Trace = new ConsoleTraceWriter(TraceLevel.Verbose);
            config.UseRedis();
            
            JobHost host = new JobHost(config);
            host.Start();

            // Give subscriber chance to startup
            Task.Delay(5000).Wait();

            //host.Call(typeof(Functions).GetMethod("SendSimplePubSubMessage"));
            //Task.Delay(5000).Wait();

            //host.Call(typeof(Functions).GetMethod("SendPubSubMessage"));
            //Task.Delay(5000).Wait();

            host.Call(typeof(Functions).GetMethod("SendPubSubMessageIdChannel"));
            Task.Delay(5000).Wait();

            //host.Call(typeof(Functions).GetMethod("AddSimpleCacheMessage"));
            //Task.Delay(5000).Wait();

            //host.Call(typeof(Functions).GetMethod("AddCacheMessage"));
            //Task.Delay(5000).Wait();

            //host.Call(typeof(Functions).GetMethod("AddCacheMessage"));
            //Task.Delay(5000).Wait();

            Console.CancelKeyPress += (sender, e) =>
            {
                host.Stop();
            };

            while (true)
            {
                Thread.Sleep(500);
            }
        }
    }
}
