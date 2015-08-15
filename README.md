# Redis.WebJobs.Extensions
Utility extensions for using Redis in Azure WebJobs.

Current implementation borrows heavily from the ServiceBus extensions in the Azure WebJobs SDK.  I'm slowly working on refactoring the implementation.

Currently I have two extensions implemented:

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

#NOTE
You will need to add your Redis and Azure Storage connection information in the App.config's of the Sample publisher and subscriber if you want to run them locally.

```
<connectionStrings>
  <add name="AzureWebJobsRedis" connectionString="[your redis name].redis.cache.windows.net,ssl=true,password=[your key],allowAdmin=true,connectTimeout=10000,syncTimeout=10000" />
  <add name="AzureWebJobsDashboard" connectionString="DefaultEndpointsProtocol=https;AccountName=[storage account name];AccountKey=[your storage key]" />
  <add name="AzureWebJobsStorage" connectionString="DefaultEndpointsProtocol=https;AccountName=[storage account name];AccountKey=[your storage key]" />
</connectionStrings>
```
