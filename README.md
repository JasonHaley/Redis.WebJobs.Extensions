# Redis.WebJobs.Extensions
Latest upgrade is to get the project to .NET Standard and ability to run in Azure Functions.

This is an extension for using Redis in Azure WebJobs and Functions.

Current implementation borrows heavily from the ServiceBus and CosmosDb extensions in the Azure WebJobs SDK and Azure WebJobs Extensions.  

There are two bindings: Redis and RedisTrigger.

#Redis
Depending on its usage is either an input or output binding that will publish a message to a Redis Channel or get/set a value in the Redis cache.  There is a Mode property that is used to determine if it is to do Pub/Sub or work with the cache.

These are in WebJobsSamples folder:

Pub/Sub example: output binding that publishes a string message on a given channel
```
 public static void SendStringMessage(
    [Redis("pubsub:stringMessages", Mode.PubSub)] out string message, 
    TextWriter log)
{
    message = "This is a test";

    log.WriteLine($"Sending message: {message}");
}
```

Pub/Sub example: output binding that publishes multiple messages on a given channel
```
public static void SendMultipleStringPubSubMessages(
    [Redis("pubsub:stringMessages", Mode.PubSub)] IAsyncCollector<string> messages, 
    TextWriter log)
{
    messages.AddAsync("Message 1");
    messages.AddAsync("Message 2");
    messages.AddAsync("Message 3");

    log.WriteLine($"Sending 3 messages");
}
```

Pub/Sub example: output binding that publishes a POCO on a given channel
```
public static void SendPocoMessage(
    [Redis("pubsub:pocoMessages", Mode.PubSub)] out Message message, 
    TextWriter log)
{
    message = new Message
    {
        Text = "This is a test POCO message",
        Sent = DateTime.UtcNow,
        Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
    };

    log.WriteLine($"Sending Message from SendPubSubMessage(): {message.Id}");
}
```

Cache example: output binding that sets a string value in the cache for a given key
```
public static void SetStringToCache(
    [Redis("StringKey", Mode.Cache)] out string value, 
    TextWriter log)
{
    value = "This is a test value at " + DateTime.UtcNow;

    log.WriteLine($"Adding String to cache from SetStringToCache(): {value}");
}
```

Cache example: output binding that uses a name resolver to get the cache key to set the
```
public static void SetStringToCacheUsingResolver(
    [Redis("%CacheKey%", Mode.Cache)] out string value, 
    TextWriter log)
{
    value = "This is a test sent using a name resolver for CacheKey at " + DateTime.UtcNow;

    log.WriteLine($"Adding String to cache from SetStringToCacheUsingResolver(): {value}");
}
```

Cache example: output binding that sets a POCO objec in the cache for a given key
```
public static void SetPocoToCache(
    [Redis("PocoKey", Mode.Cache)] out Message message, 
    TextWriter log)
{
    message = new Message
    {
        Text = "message #",
        Sent = DateTime.UtcNow,
        Id = Guid.Parse("bc3a6131-937c-4541-a0cf-27d49b96a5f2")
    };

    log.WriteLine($"Adding Message to cache from SetPocoToCache(): {message.Id}");
}
```

#RedisTrigger
This trigger has two modes: 

PubSub - the trigger will subscribe to a Redis Channel, passing messages into the function as they are published.

Cache - the trigger will check the value of a redis cache key on a determined TimeSpan frequency and compare the value to the previous checked value and will call function if values are different.  Uses a string compare on json serialized objects.

Pub/Sub example: Trigger that listens for string messages
```
public static void ReceiveStringMessage(
    [RedisTrigger("pubsub:stringMessages", Mode.PubSub)] string message, 
    TextWriter log)
{
    log.WriteLine($"--- Received String Message: {message} ---");
}
```

Pub/Sub example: Trigger that listens for POCO messages
```
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
```

Pub/Sub example: Trigger that listens for POCO message, uses {Id} binding from that messages to set output binding channel to publish string message on.
```
public static void EchoMessageUsingResolver(
    [RedisTrigger("pubsub:pocoMessages")] Message message,
    [Redis("pubsub:{Id}", Mode.PubSub)] IAsyncCollector<string> messages, 
    TextWriter log)
{
    message.Text = "This is a test POCO message ECHO ECHO ECHO";
    messages.AddAsync(message.Text);

    log.WriteLine($"Sending Message from SendPocoMessageUsingResolver(): {message.Id}");
}
```

Pub/Sub example: Trigger that listens to a wildcard channel name
```
public static void ReceiveAllMessages(
    [RedisTrigger("pubsub:*")] string message, 
    TextWriter log)
{
    log.WriteLine($"+++ All Messages: Received Message - {message} +++");
}
```

Cache example: Trigger that gets the value of a cache key, value is received if it has changed in the last 30 seconds.  
- CheckCacheFrequency is set in the RedisConfiguration (default is 30 seconds)
```
public static void GetStringCache(
    [RedisTrigger("StringKey", Mode.Cache)] string lastMessage, 
    TextWriter log)
{
    log.WriteLine($"StringKey retrieved: {lastMessage}");
}
```

Cache example: Trigger that gets the value of a cache key as a POCO object, value is received if it has changed in the last 30 seconds.  
- CheckCacheFrequency is set in the RedisConfiguration (default is 30 seconds)
```
public static void GetPocoCache(
    [RedisTrigger("PocoKey", Mode.Cache)] Message lastMessage, 
    TextWriter log)
{
    log.WriteLine($"PocoKey retrieved. Id: {lastMessage.Id} Text: {lastMessage.Text}");
}
```

Cache example: Trigger that gets a cache value using the key from a name resolver
```
public static void GetStringCacheUsingNameResolver(
    [RedisTrigger("%CacheKey%", Mode.Cache)] string lastValue, 
    TextWriter log)
{
    log.WriteLine($"CacheKey name resolver retrieved: {lastValue}");
}
```

Cache example: input binding that gets the value of a cache key, uses a TimerTrigger to check
```
public static void GetStringValueFromCache(
    [TimerTrigger("00:01", RunOnStartup = true)] TimerInfo timer,
    [Redis("StringKey", Mode.Cache)] string message,
    TextWriter log)
{
    log.WriteLine($"Getting String in cache from GetStringValueFromCache(): {message}");
}
```

Cache example: input/output binding that gets a POCO object from the cache for a key and updates the cache value on exit.  Uses a TimerTigger to check the cache value.
```
public static void GetSetPocoValueFromCache(
    [TimerTrigger("00:01", RunOnStartup = true)] TimerInfo timer,
    [Redis("PocoKey", Mode.Cache)] Message message,
    TextWriter log)
{
    counter = +1;
    message.Text = $"{message.Text}..{counter}";

    log.WriteLine($"Getting and Setting Poco in cache from GetSetPocoValueFromCache(): {message.Text}");
}
```

#NOTE
You will need to add your Redis and Azure Storage connection information in the AppSettings in Azure or if using the local development you can add as environment variables or with the function app create a local.settings.config.  Here is a sample to start with for the function app to run locally:

```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsEnv": "Development",
        "AzureWebJobsRedisConnectionString": "[your redis db].redis.cache.windows.net:6380,password=[your password]],ssl=True,abortConnect=False",
        "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=[your storage account];AccountKey=[your account key];EndpointSuffix=core.windows.net",
        "AzureWebJobsDashboard": "DefaultEndpointsProtocol=https;AccountName=[your storage account];AccountKey=[your account key];EndpointSuffix=core.windows.net"
    }
}
```
