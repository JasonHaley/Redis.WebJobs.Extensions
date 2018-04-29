using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions;

namespace FunctionApp
{
    public static class ReceiveMessageFunction
    {
        [FunctionName("ReceiveMessage")]
        public static void Run([RedisTrigger("pubsub:messages", Mode.PubSub)] string message, TextWriter log)
        {
            log.WriteLine($"Received Message: {message}");
        }
    }
}
