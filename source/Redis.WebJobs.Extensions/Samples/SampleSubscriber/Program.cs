using System;
using System.Diagnostics;
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;
using Redis.WebJobs.Extensions.Config;
using Redis.WebJobs.Extensions.Framework;

namespace SampleSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();
            //config.Tracing.Trace = new ConsoleTraceWriter(TraceLevel.Verbose);
            config.UseRedis();
            
            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
