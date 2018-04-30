using Microsoft.Azure.WebJobs;
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

        // Cache Examples ***********************************************************************************
        public static void GetStringCache([RedisTrigger("StringKey", Mode.Cache)] string lastMessage, TextWriter log)
        {
            log.WriteLine($"StringKey retrieved: {lastMessage}");
        }

        public static void GetPocoCache([RedisTrigger("PocoKey", Mode.Cache)] Message lastMessage, TextWriter log)
        {
            log.WriteLine($"PocoKey retrieved. Id: {lastMessage.Id} Text: {lastMessage.Text}");
        }

        //public static void GetCacheTriggerMessageById([RedisTrigger("LastMessage:bc3a6131-937c-4541-a0cf-27d49b96a5f2", Mode.Cache)] Message lastMessage, TextWriter log)
        //{
        //    log.WriteLine($"LastMessage retrieved. Id: {lastMessage.Id} Text: {lastMessage.Text}");
        //}
        
        private static int counter = 0;
        public static void GetStringValueFromCache([TimerTrigger("00:01", RunOnStartup = true)] TimerInfo timer,
            [Redis("StringKey", Mode.Cache)] string message,
            TextWriter log)
        {
            counter =+ 1;
            message = $"{message}..{counter}";

            log.WriteLine($"Adding String to cache from SetStringToCache(): {message}");
        }

        public static void GetPocoValueFromCache([TimerTrigger("00:01", RunOnStartup = true)] TimerInfo timer,
            [Redis("PocoKey", Mode.Cache)] Message message,
            TextWriter log)
        {
            counter = +1;
            message.Text = $"{message.Text}..{counter}";

            log.WriteLine($"Adding String to cache from SetStringToCache(): {message.Text}");
        }
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
