using System;
using System.IO;
using Redis.WebJobs.Extensions;

namespace SamplePublisher
{
    public static class Functions
    {
        public static void SendSimpleMessage([RedisPublish("messages")] out string message, TextWriter log)
        {
            message = "this is a test";

            log.WriteLine("sending message: " + message);
        }

        public static void SendMessage([RedisPublish("messages")] out Message message, TextWriter log)
        {
            message = new Message
            {
                Text = "message #",
                Sent = DateTime.UtcNow,
                Id = Guid.NewGuid()
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
