using System.IO;
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;

namespace FunctionApp
{
    public static class GetPocoFromCacheFunction
    {
        [FunctionName("GetPocoFromCache")]
        public static void Run([RedisTrigger("cache:pocoKey", Mode.Cache)] Message message, TextWriter log)
        {
            log.WriteLine($"Retrieved Cache Message: {message.Id} Sent: {message.Sent}");
        }
    }
}
