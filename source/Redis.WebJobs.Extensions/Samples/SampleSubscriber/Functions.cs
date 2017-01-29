using System;
using System.IO;
using Redis.WebJobs.Extensions;

namespace SampleSubscriber
{
    public static class Functions
    {
        // PubSub Examples ***********************************************************************************
        public static void ReceivePubSubSimpleMessage([RedisTrigger("pubsub:simpleMessages", Mode.PubSub)] string message, TextWriter log)
        {
            log.WriteLine($"Received Message: {message}");
        }

        public static void ReceivePubSubMessage([RedisTrigger("pubsub:messages")] Message message, TextWriter log)
        {

            if (message != null)
            {
                log.WriteLine($"***ReceivedMessage: {message.Text} Sent: {message.Sent}");
            }
            else
            {
                log.WriteLine("***ReceivedMessage: message sent but not compatible withe Message type");
            }
        }

        public static void ReceivePubSubMessageIdChannel([RedisTrigger("pubsub:messages:bc3a6131-937c-4541-a0cf-27d49b96a5f2")] Message message, TextWriter log)
        {

            if (message != null)
            {
                log.WriteLine($"***ReceivedMessage: {message.Text} Sent: {message.Sent}");
            }
            else
            {
                log.WriteLine("***ReceivedMessage: message sent but not compatible withe Message type");
            }
        }

        public static void ReceiveAllPubSubMessage([RedisTrigger("pubsub:messages:*")] Message message, TextWriter log)
        {
            if (message != null)
            {
                log.WriteLine($"***ReceivedMessage: {message.Text} Sent: {message.Sent}");
            }
            else
            {
                log.WriteLine("***ReceivedMessage: message sent but not compatible withe Message type");
            }
        }

        // Cache Examples ***********************************************************************************
        public static void GetCacheTriggerSimpleMessage([RedisTrigger("LastSimpleMessage", Mode.Cache)] string lastMessage, TextWriter log)
        {
            log.WriteLine($"LastSimpleMessage retrieved: {lastMessage}");
        }

        public static void GetCacheTriggerMessage([RedisTrigger("LastMessage", Mode.Cache)] Message lastMessage, TextWriter log)
        {
            log.WriteLine($"LastMessage retrieved. Id: {lastMessage.Id} Text: {lastMessage.Text}");
        }


        public static void GetCacheTriggerMessageById([RedisTrigger("LastMessage:bc3a6131-937c-4541-a0cf-27d49b96a5f2", Mode.Cache)] Message lastMessage, TextWriter log)
        {
            log.WriteLine($"LastMessage retrieved. Id: {lastMessage.Id} Text: {lastMessage.Text}");
        }
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
