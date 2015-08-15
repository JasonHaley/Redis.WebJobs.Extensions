using System;
using System.IO;
using Redis.WebJobs.Extensions;

namespace SampleSubscriber
{
    public static class Functions
    {
        public static void ReceiveSimpleMessage([RedisSubscribeTrigger("messages")] string message)
        {
            Console.WriteLine("Received Message: {0}", message);
        }

        public static void ReceiveMessage([RedisSubscribeTrigger("messages")] Message message, TextWriter log)
        {

            if (message != null)
            {
                log.WriteLine("***ReceivedMessage: {0} Sent: {1}", message.Text, message.Sent);
            }
            else
            {
                log.WriteLine("***ReceivedMessage: message sent but not compatible withe Message type");
            }

        }
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
