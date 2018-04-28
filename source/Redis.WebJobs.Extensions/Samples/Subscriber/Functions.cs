using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions;
using System;
using System.IO;

namespace Subscriber
{
    public static class Functions
    {
        //public static void ReceivePubSubSimpleMessage([RedisTrigger("pubsub:simpleMessages", Mode.PubSub)] string message, TextWriter log)
        //{
        //    log.WriteLine($"Received Message: {message}");
        //}

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
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
