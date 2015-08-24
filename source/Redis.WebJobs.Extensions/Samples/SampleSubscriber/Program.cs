using System;
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;
using Redis.WebJobs.Extensions.Config;

namespace SampleSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();
            
            config.UseRedis();
            
            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
