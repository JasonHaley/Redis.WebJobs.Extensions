using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Redis.WebJobs.Extensions;

namespace FunctionApp
{
    public static class SetPocoCacheFunction
    {
        [FunctionName("SetPocoCache")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req,
            [Redis("cache:pocoKey", Mode.Cache)] IAsyncCollector<Message> messages, TextWriter log)
        {
            string message = req.Query["message"];

            if (string.IsNullOrEmpty(message))
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                message = message ?? data?.message;
            }

            var poco = new Message() { Id = 1, Sent = DateTime.UtcNow, Text = message };
            messages.AddAsync(poco);

            log.WriteLine($"Sending message: {poco.Text} at: {poco.Sent}");

            return message != null
                ? (ActionResult)new OkObjectResult($"Message sent: {message}")
                : new BadRequestObjectResult("Please pass a message on the query string or in the request body");
        }
    }
}
