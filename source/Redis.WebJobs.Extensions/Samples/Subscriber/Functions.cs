using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;
using System;
using System.IO;

namespace Subscriber
{
    public static class Functions
    {
        // Pub/Sub example: Trigger that listens for string messages
        public static void ReceiveStringMessage(
            [RedisTrigger("pubsub:stringMessages", Mode.PubSub)] string message,
            TextWriter log)
        {
            log.WriteLine($"--- Received String Message: {message} ---");
        }

        // Pub/Sub example: Trigger that listens for POCO messages
        public static void ReceivePocoMessage(
            [RedisTrigger("pubsub:pocoMessages")] Message message,
            TextWriter log)
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

        // Pub/Sub example: Trigger that listens for POCO message, uses {Id} binding from that messages
        //   to set output binding channel to publish string message on.
        public static void EchoMessageUsingResolver(
            [RedisTrigger("pubsub:pocoMessages")] Message message,
            [Redis("pubsub:{Id}", Mode.PubSub)] IAsyncCollector<string> messages,
            TextWriter log)
        {
            message.Text = "This is a test POCO message ECHO ECHO ECHO";
            messages.AddAsync(message.Text);

            log.WriteLine($"Sending Message from SendPocoMessageUsingResolver(): {message.Id}");
        }

        // Pub/Sub example: Trigger that listens to a wildcard channel name
        public static void ReceiveAllMessages(
            [RedisTrigger("pubsub:*")] string message,
            TextWriter log)
        {
            log.WriteLine($"+++ All Messages: Received Message - {message} +++");
        }

        // Cache Examples ***********************************************************************************

        // Cache example: Trigger that gets the value of a cache key, value is received if it has changed
        //   in the last 10 seconds.  Value will be compared to previous value every 10 seconds
        //   - CheckCacheFrequency is set in the Program.cs (default is 30 seconds)
        public static void GetStringCache(
            [RedisTrigger("StringKey", Mode.Cache)] string lastMessage,
            TextWriter log)
        {
            log.WriteLine($"StringKey retrieved: {lastMessage}");
        }

        // Cache example: Trigger that gets the value of a cache key, value is received if it has changed
        //    in the last 10 seconds.  Value will be compared to previous value every 10 seconds
        //   - CheckCacheFrequency is set in the Program.cs (default is 30 seconds)
        public static void GetPocoCache(
            [RedisTrigger("PocoKey", Mode.Cache)] Message lastMessage,
            TextWriter log)
        {
            log.WriteLine($"PocoKey retrieved. Id: {lastMessage.Id} Text: {lastMessage.Text}");
        }

        // Cache example: Trigger that gets a cache value using the key from a name resolver
        public static void GetStringCacheUsingNameResolver(
            [RedisTrigger("%CacheKey%", Mode.Cache)] string lastValue,
            TextWriter log)
        {
            log.WriteLine($"CacheKey name resolver retrieved: {lastValue}");
        }

        private static int counter = 0;

        // Cache example: input binding that gets the value of a cache key, uses a TimerTrigger to check 
        //    the cache value
        public static void GetStringValueFromCache(
            [TimerTrigger("00:01", RunOnStartup = true)] TimerInfo timer,
            [Redis("StringKey", Mode.Cache)] string message,
            TextWriter log)
        {
            log.WriteLine($"Getting String in cache from GetStringValueFromCache(): {message}");
        }


        // Cache example: input/output binding that gets a POCO object from the cache for a key and 
        //    updates the cache value on exit.  Uses a TimerTigger to check the cache value.
        public static void GetSetPocoValueFromCache(
            [TimerTrigger("00:01", RunOnStartup = true)] TimerInfo timer,
            [Redis("PocoKey", Mode.Cache)] Message message,
            TextWriter log)
        {
            counter = +1;
            message.Text = $"{message.Text}..{counter}";

            log.WriteLine($"Getting and Setting Poco in cache from GetSetPocoValueFromCache(): {message.Text}");
        }

        
    }

    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Sent { get; set; }
    }
}
