# Redis.WebJobs.Extensions
Utility extensions for using Redis in Azure WebJobs.

Current implementation borrows heavily from the ServiceBus extensions in the Azure WebJobs SDK.  I'm slowly working on refactoring the implementation.

There are two bindings: Redis and RedisTrigger.

#Redis
A binding that will publish a message to a Redis Channel.

Example of sending a string message
```
public static void SendSimpleMessage([Redis("messages", Mode.PubSub)] out string message, TextWriter log)
{
  message = "this is a test";

  log.WriteLine("sending message: " + message);
}
```

Exmple of sending an object that can be serialized with the Newtonsoft.Json serializer.
```
public static void SendMessage([Redis("messages", Mode.PubSub)] out Message message, TextWriter log)
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

Example of putting a string into a redis cache:
```
public static void ReceiveAndStoreSimpleMessage([RedisTrigger("messages", Mode.PubSub)] string message,
  [Redis("LastSimpleMessage", Mode.Cache)] out string lastMessage, TextWriter log)
{
  lastMessage = message;
  log.WriteLine("Last message received: " + message);
  log.WriteLine("Storing with key: LastSimpleMessage");
}
```

Example of putting an object into a redis cache:
```
public static void ReceiveAndStoreMessage([RedisTrigger("messages", Mode.PubSub)] Message message,
  [Redis("LastMessage", Mode.Cache)] out Message lastMessage, TextWriter log)
{
  lastMessage = message;
  log.WriteLine("Last message id received: " + message.Id);
  log.WriteLine("Storing with key: LastMessage");
}
```

Example of retrieving a string value out of a redis cache:
```
public static void GetSimpleMessage([RedisTrigger("messages", Mode.PubSub)] string message, 
  [Redis("LastSimpleMessage", Mode.Cache)] string lastMessage, TextWriter log)
{
  log.WriteLine("LastSimpleMessage retrieved: " + lastMessage);
}
```

Example of retrieving an object out of a redis cache:
```
public static void GetMessage([RedisTrigger("messages", Mode.PubSub)] string message, 
  [Redis("LastMessage", Mode.Cache)] Message lastMessage, TextWriter log)
{
  log.WriteLine("LastMessage retrieved. Id:" + lastMessage.Id + " Text:" + lastMessage.Text);
}
```

#RedisTrigger
A trigger that subscribes to a Redis Channel, passing in messages as they are published.

Example of receiving string messages
```
public static void ReceiveSimpleMessage([RedisTrigger("messages", Mode.PubSub)] string message)
{
    Console.WriteLine("Received Message: {0}", message);
}
```

Example of receiving deserialized json objects in message
```
public static void ReceiveMessage([RedisTrigger("messages", Mode.PubSub)] Message message, TextWriter log)
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

Example of receiving a string messages when cache value for a given key has changed
```
public static void GetCacheTriggerSimpleMessage([RedisTrigger("LastSimpleMessage", Mode.Cache)] string lastMessage, 
  TextWriter log)
{
  log.WriteLine("LastSimpleMessage retrieved: " + lastMessage);
}
```

Example of receiving a deserialized json object when a cache value for a given key has changed (uses a string comparison to determine value change)
```
public static void GetCacheTriggerMessage([RedisTrigger("LastMessage", Mode.Cache)] Message lastMessage, 
  TextWriter log)
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
