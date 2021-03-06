using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Redis.WebJobs.Extensions;

namespace FunctionApp
{
    public static class SendMessageFunction
    {
        [FunctionName("SendMessage")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req,
            [Redis("pubsub:messages", Mode.PubSub)] IAsyncCollector<string> messages, TextWriter log)
        {
            string message = req.Query["message"];

            if (string.IsNullOrEmpty(message))
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                message = requestBody;
            }

            messages.AddAsync(message);

            log.WriteLine($"Sending message: {message}");

            return message != null
                ? (ActionResult)new OkObjectResult($"Message sent: {message}")
                : new BadRequestObjectResult("Please pass a message on the query string or in the request body");
        }
    }
}
