using System;
using System.IO;
using Redis.WebJobs.Extensions;

namespace SampleSubscriber
{
    public static class Functions
    {

        public static void ReceivePubSubSimpleMessage([RedisTrigger("simpleMessages", Mode.PubSub)] string message, TextWriter log)
        {
            log.WriteLine("Received Message: {0}", message);
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

        public static void ReceivePubSubMessageIdChannel([RedisTrigger("messages:bc3a6131-937c-4541-a0cf-27d49b96a5f2")] Message message, TextWriter log)
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

        public static void ReceiveAllPubSubMessage([RedisTrigger("messages:*")] Message message, TextWriter log)
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
        }

        public static void GetCacheMessage([RedisTrigger("messages", Mode.PubSub)] string message, [Redis("LastMessage", Mode.Cache)] Message lastMessage, TextWriter log)
        {
            log.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);
        }

        public static void GetCacheTriggerSimpleMessage([RedisTrigger("LastSimpleMessage", Mode.Cache)] string lastMessage, TextWriter log)
        {
            log.WriteLine("LastSimpleMessage retrieved: " + lastMessage);
        }

        public static void GetCacheTriggerMessage([RedisTrigger("LastMessage:bc3a6131-937c-4541-a0cf-27d49b96a5f2", Mode.Cache)] Message lastMessage, TextWriter log)
        {
            log.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);
        }
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
