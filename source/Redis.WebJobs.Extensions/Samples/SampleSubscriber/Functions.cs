using System;
using System.IO;
using Redis.WebJobs.Extensions;

namespace SampleSubscriber
{
    public static class Functions
    {
        public static void ReceiveSimpleMessage([RedisSubscribeTrigger("messages")] string message)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("Received Message: {0}", message);

            Console.ForegroundColor = defaultColor;
        }

        public static void ReceiveMessage([RedisSubscribeTrigger("messages")] Message message, TextWriter log)
        {
            string outMessage;
            if (message != null)
            {
                outMessage = string.Format("***ReceivedMessage: {0} Sent: {1}", message.Text, message.Sent);
            }
            else
            {
                outMessage = "***ReceivedMessage: message sent but not compatible withe Message type";
            }

            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine(outMessage);

            Console.ForegroundColor = defaultColor;

            log.WriteLine(outMessage);

        }

        public static void ReceiveSimpleMessageWithWildcard([RedisSubscribeTrigger("messages:*")] string message)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Received Message: {0}", message);
            Console.ForegroundColor = defaultColor;
        }

        public static void ReceiveMessageWithWildcard([RedisSubscribeTrigger("messages:*")] Message message, TextWriter log)
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

        //public static void ReceiveAndStoreSimpleMessage([RedisSubscribeTrigger("messages")] string message,
        //    [RedisAddOrReplace("LastSimpleMessage")] out string lastMessage, TextWriter log)
        //{
        //    lastMessage = message;

        //    log.WriteLine("Last message received: " + message);
        //    log.WriteLine("Storing with key: LastSimpleMessage");
        //}

        //public static void ReceiveAndStoreMessage([RedisSubscribeTrigger("messages")] Message message,
        //    [RedisAddOrReplace("LastMessage")] out Message lastMessage, TextWriter log)
        //{
        //    lastMessage = message;

        //    log.WriteLine("Last message id received: " + message.Id);
        //    log.WriteLine("Storing with key: LastMessage");
        //}

        //public static void ReceiveAndStoreMessageWithDynamicKey([RedisSubscribeTrigger("messages")] Message message,
        //    [RedisAddOrReplace("Messages:{Id}")] out Message lastMessage, TextWriter log)
        //{
        //    lastMessage = message;

        //    log.WriteLine("Last message id received: " + message.Id);
        //    log.WriteLine("Storing with key: Messages:" + message.Id);
        //}

        //public static void GetSimpleMessage([RedisGet("LastSimpleMessage")] string lastMessage, TextWriter log)
        //{
        //    log.WriteLine("LastSimpleMessage retrieved: " + lastMessage);
        //}

        //public static void GetMessage([RedisGet("LastMessage")] Message lastMessage, TextWriter log)
        //{
        //    log.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);
        //}
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
