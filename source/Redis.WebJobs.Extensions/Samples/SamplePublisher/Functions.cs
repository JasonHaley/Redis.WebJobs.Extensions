using System;
using System.IO;
using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions;

namespace SamplePublisher
{
    public static class Functions
    {
        // PubSub Examples ***********************************************************************************
        public static void SendSimplePubSubMessage([Redis("pubsub:simpleMessages", Mode.PubSub)] out string message, TextWriter log, TraceWriter trace)
        {
            message = "This is a test";

            log.WriteLine($"Sending message: {message}");

            trace.Info($"New message sent SendSimplePubSubMessage(): {message}");
        }
        public static void SendPubSubMessage([Redis("pubsub:messages", Mode.PubSub)] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "message #",
                Sent = DateTime.UtcNow,
                Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
            };

            log.WriteLine($"Sending Message from SendPubSubMessage(): {message.Id}");
        }

        public static void SendPubSubMessageIdChannel([Redis("pubsub:messages:{Id}", Mode.PubSub)] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "message #",
                Sent = DateTime.UtcNow,
                Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
            };

            log.WriteLine($"Sending Message from SendPubSubMessageIdChannel(): {message.Id}");
        }

        // Cache Examples ***********************************************************************************
        public static void AddSimpleCacheMessage([Redis("LastSimpleMessage", Mode.Cache)] out string message, TextWriter log)
        {
            message = "This is a test";

            log.WriteLine($"Adding Message to cache from AddSimpleCacheMessage(): {message}");
        }

        public static void AddCacheLastMessage([Redis("LastMessage", Mode.Cache)] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "message #",
                Sent = DateTime.UtcNow,
                Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
            };

            log.WriteLine($"Adding Message to cache from AddCacheLastMessage(): {message.Id}");
        }

        public static void AddCacheMessage([Redis("LastMessage:{Id}", Mode.Cache)] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "message #",
                Sent = DateTime.UtcNow,
                Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
            };

            log.WriteLine($"Adding Message to cache from AddCacheMessage(): {message.Id}");
        }

        public class Message
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public DateTime Sent { get; set; }
        }
    }
}
