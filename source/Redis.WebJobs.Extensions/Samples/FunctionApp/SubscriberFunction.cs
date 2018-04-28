using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions;

namespace FunctionApp
{
    public static class SubscriberFunction
    {
        [FunctionName("SubscriberFunction")]
        public static void ReceivePubSubSimpleMessage([RedisTrigger("pubsub:simpleMessages", Mode.PubSub)] string message, TextWriter log, TraceWriter trace)
        {
            log.WriteLine($"Received Message: {message}");
            trace.Info($"New message received ReceivePubSubSimpleMessage(): {message}");
            Console.Write($"Received message: {message}");
        }
    }
}
