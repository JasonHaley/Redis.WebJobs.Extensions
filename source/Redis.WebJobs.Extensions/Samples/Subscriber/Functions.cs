using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions;
using System;
using System.IO;

namespace Subscriber
{
    public static class Functions
    {
        public static void ReceivePubSubSimpleMessage([RedisTrigger("pubsub:simpleMessages", Mode.PubSub)] string message, TextWriter log, TraceWriter trace)
        {
            log.WriteLine($"Received Message: {message}");
            trace.Info($"New message received ReceivePubSubSimpleMessage(): {message}");
            Console.Write($"Received message: {message}");
        }
    }
}
