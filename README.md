# Redis.WebJobs.Extensions
Utility extensions for using Redis in Azure WebJobs.

Current implementation borrows heavily from the ServiceBus extensions in the Azure WebJobs SDK.  I'm slowly working on refactoring the implementation.

Currently I have four extensions implemented: RedisPublish, RedisSubscribe, RedisAddOrReplace, RedisGet.

#RedisPublish
A binding that will publish a message to a Redis Channel.

Example of sending a string message
```
public static void SendSimpleMessage([RedisPublish("messages")] out string message, TextWriter log)
{
  message = "this is a test";

  log.WriteLine("sending message: " + message);
}
```

Exmple of sending an object that can be serialized with the Newtonsoft.Json serializer.
```
public static void SendMessage([RedisPublish("messages")] out Message message, TextWriter log)
{
  message = new Message
  {
    Text = "message #",
    Sent = DateTime.UtcNow,
    Id = Guid.NewGuid()
  };

  log.WriteLine("sending Message: " + message.Id);
}
```

#RedisSubscribeTrigger
A trigger that subscribes to a Redis Channel, passing in messages as they are published.

Example of receiving string messages
```
public static void ReceiveSimpleMessage([RedisSubscribeTrigger("messages")] string message)
{
    Console.WriteLine("Received Message: {0}", message);
}
```

Example of receiving serialized json objects in message
```
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
```

#RedisAddOrReplace
A binding that will add or replace a value for a given key in redis.  If the object is not a string it needs to be Json serializable to be successfully stored.

Example of putting a string into a redis cache:
```
public static void ReceiveAndStoreSimpleMessage([RedisSubscribeTrigger("messages")] string message,
  [RedisAddOrReplace("LastSimpleMessage")] out string lastMessage, TextWriter log)
{
  lastMessage = message;
  log.WriteLine("Last message received: " + message);
  log.WriteLine("Storing with key: LastSimpleMessage");
}
```

Example of putting an object into a redis cache:
```
public static void ReceiveAndStoreMessage([RedisSubscribeTrigger("messages")] Message message,
  [RedisAddOrReplace("LastMessage")] out Message lastMessage, TextWriter log)
{
  lastMessage = message;
  log.WriteLine("Last message id received: " + message.Id);
  log.WriteLine("Storing with key: LastMessage");
}
```

#RedisGet
Retrieves a value from a redis cache.  If the value is not a string, it needs to be Json serializable.

Example of retrieving a string value out of a redis cache:
```
public static void GetSimpleMessage([RedisSubscribeTrigger("messages")] string message, 
  [RedisGet("LastSimpleMessage")] string lastMessage, TextWriter log)
{
  log.WriteLine("LastSimpleMessage retrieved: " + lastMessage);
}
```

Example of retrieving an object out of a redis cache:
```
public static void GetMessage([RedisSubscribeTrigger("messages")] string message, 
  [RedisGet("LastMessage")] Message lastMessage, TextWriter log)
{
  log.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);
}
```

#NOTE
You will need to add your Redis and Azure Storage connection information in the App.config's of the Sample publisher and subscriber if you want to run them locally.

```
<connectionStrings>
  <add name="AzureWebJobsRedis" connectionString="[your redis name].redis.cache.windows.net,ssl=true,password=[your key],allowAdmin=true,connectTimeout=10000,syncTimeout=10000" />
  <add name="AzureWebJobsDashboard" connectionString="DefaultEndpointsProtocol=https;AccountName=[storage account name];AccountKey=[your storage key]" />
  <add name="AzureWebJobsStorage" connectionString="DefaultEndpointsProtocol=https;AccountName=[storage account name];AccountKey=[your storage key]" />
</connectionStrings>
```
