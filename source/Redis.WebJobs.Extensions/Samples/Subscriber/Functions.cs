using Redis.WebJobs.Extensions;
using System;
using System.IO;

namespace Subscriber
{
    public static class Functions
    {
        public static void ReceiveStringMessage([RedisTrigger("pubsub:stringMessages", Mode.PubSub)] string message, TextWriter log)
        {
            log.WriteLine($"--- Received String Message: {message} ---");
        }

        public static void ReceivePocoMessage([RedisTrigger("pubsub:pocoMessages")] Message message, TextWriter log)
        {

            if (message != null)
            {
                log.WriteLine($"*** Received Poco Message: {message.Text} Sent: {message.Sent} ***");
            }
            else
            {
                log.WriteLine("*** Received Poco Message: message sent but not compatible withe Message type ***");
            }
        }

        public static void ReceiveAllMessages([RedisTrigger("pubsub:*")] string message, TextWriter log)
        {
            log.WriteLine($"+++ All Messages: Received Message - {message} +++");
        }
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
