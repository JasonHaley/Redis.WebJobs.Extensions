using System.IO;
using Microsoft.Azure.WebJobs;
using Redis.WebJobs.Extensions;

namespace FunctionApp
{
    public static class GetStringFromCacheFunction
    {
        [FunctionName("GetStringFromCache")]
        public static void Run([RedisTrigger("cache:stringKey", Mode.Cache)] string value, TextWriter log)
        {
            log.WriteLine($"Retrieved Cache Value: {value}");
        }
    }
}
