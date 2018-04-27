
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions;
using System;
using System.IO;

namespace Publisher
{
    public static class Functions
    {
        // PubSub Examples ***********************************************************************************
        public static void SendSimplePubSubMessage([Redis("pubsub:simpleMessages", Mode.PubSub)] out string message, TextWriter log, TraceWriter trace)
        {
            message = "This is a test";

            log.WriteLine($"Sending message: {message}");
            trace.Info($"New message sent SendSimplePubSubMessage(): {message}");
            Console.Write($"Sending message: {message}");
        }

        public static void SendMultipleSimplePubSubMessages([Redis("pubsub:simpleMessages", Mode.PubSub)] IAsyncCollector<string> messages, TextWriter log, TraceWriter trace)
        {
            messages.AddAsync("Message 1");
            messages.AddAsync("Message 2");
            messages.AddAsync("Message 3");

            log.WriteLine($"Sending 3 messages");
            trace.Info($"New messages sent SendMultipleSimplePubSubMessages(): 3");
            Console.Write($"Sending 3 messages");
        }
    }
}
