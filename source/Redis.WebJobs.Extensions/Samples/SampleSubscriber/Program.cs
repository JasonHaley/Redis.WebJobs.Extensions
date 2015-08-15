using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;

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
