using System;
using System.IO;
using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions;

namespace SamplePublisher
{
    public static class Functions
    {
        public static void SendSimplePubSubMessage([Redis("simpleMessages", Mode.PubSub)] out string message, TextWriter log, TraceWriter trace)
        {
            message = "this is a test";

            log.WriteLine("sending message: " + message);

            trace.Info(string.Format("New message sent: {0}", message));
        }
        public static void SendPubSubMessage([Redis("messages", Mode.PubSub)] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "message #",
                Sent = DateTime.UtcNow,
                Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
            };

            log.WriteLine("sending Message: " + message.Id);
        }

        public static void SendPubSubMessageIdChannel([Redis("messages:{Id}", Mode.PubSub)] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "message #",
                Sent = DateTime.UtcNow,
                Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
            };

            log.WriteLine("sending Message: " + message.Id);
        }

        public static void AddSimpleCacheMessage([Redis("LastSimpleMessage", Mode.Cache)] out string message, TextWriter log)
        {
            message = "this is a test";

            log.WriteLine("sending message: " + message);
        }

        public static void AddCacheMessage([Redis("LastMessage:{Id}", Mode.Cache)] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "message #",
                Sent = DateTime.UtcNow,
                Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
            };

            log.WriteLine("sending Message: " + message.Id);
        }

        public class Message
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public DateTime Sent { get; set; }
        }
    }
}
