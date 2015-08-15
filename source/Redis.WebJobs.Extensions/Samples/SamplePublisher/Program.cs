using System;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;

namespace SamplePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();

            config.UseRedis();
            
            JobHost host = new JobHost(config);
            host.Start();

            host.Call(typeof(Functions).GetMethod("SendSimpleMessage"));
            host.Call(typeof(Functions).GetMethod("SendMessage"));

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
