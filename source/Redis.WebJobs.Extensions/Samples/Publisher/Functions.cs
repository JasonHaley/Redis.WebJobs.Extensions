using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;
using System;
using System.IO;

namespace Publisher
{
    public static class Functions
    {
        // PubSub Examples ***********************************************************************************
        public static void SendStringMessage([Redis("pubsub:stringMessages", Mode.PubSub)] out string message, TextWriter log)
        {
            message = "This is a test";

            log.WriteLine($"Sending message: {message}");
        }

        public static void SendMultipleStringPubSubMessages([Redis("pubsub:stringMessages", Mode.PubSub)] IAsyncCollector<string> messages, TextWriter log)
        {
            messages.AddAsync("Message 1");
            messages.AddAsync("Message 2");
            messages.AddAsync("Message 3");

            log.WriteLine($"Sending 3 messages");
        }

        public static void SendPocoMessage([Redis("pubsub:pocoMessages", Mode.PubSub)] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "This is a test POCO message",
                Sent = DateTime.UtcNow,
                Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
            };

            log.WriteLine($"Sending Message from SendPubSubMessage(): {message.Id}");
        }


        public class Message
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public DateTime Sent { get; set; }
        }
    }
}
