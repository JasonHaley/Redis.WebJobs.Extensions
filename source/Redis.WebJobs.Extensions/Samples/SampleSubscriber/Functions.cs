using System;
using System.IO;
using Redis.WebJobs.Extensions;

namespace SampleSubscriber
{
    public static class Functions
    {
        
        public static void ReceivePubSubSimpleMessage([RedisTrigger("messages", Mode.PubSub)] string message)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Received Message: {0}", message);
            Console.ForegroundColor = defaultColor;
        }

        public static void ReceivePubSubMessage([RedisTrigger("messages")] Message message, TextWriter log)
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

        public static void GetCacheSimpleMessage([RedisTrigger("messages", Mode.PubSub)] string message, [Redis("LastSimpleMessage", Mode.Cache)] string lastMessage, TextWriter log)
        {
            log.WriteLine("LastSimpleMessage retrieved: " + lastMessage);

            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("LastSimpleMessage retrieved: " + lastMessage);

            Console.ForegroundColor = defaultColor;
        }

        public static void GetCacheMessage([RedisTrigger("messages", Mode.PubSub)] string message, [Redis("LastMessage", Mode.Cache)] Message lastMessage, TextWriter log)
        {
            log.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);

            // ------------------------------------------------------------------------
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);

            Console.ForegroundColor = defaultColor;
            // ------------------------------------------------------------------------
        }

        public static void GetCacheTriggerSimpleMessage([RedisTrigger("LastSimpleMessage", Mode.Cache)] string lastMessage, TextWriter log)
        {
            log.WriteLine("LastSimpleMessage retrieved: " + lastMessage);

            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("LastSimpleMessage retrieved: " + lastMessage);

            Console.ForegroundColor = defaultColor;
        }

        public static void GetCacheTriggerMessage([RedisTrigger("LastMessage", Mode.Cache)] Message lastMessage, TextWriter log)
        {
            log.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);

            // ------------------------------------------------------------------------
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);

            Console.ForegroundColor = defaultColor;
            // ------------------------------------------------------------------------
        }
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
