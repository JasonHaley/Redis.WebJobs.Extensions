
namespace Redis.WebJobs.Extensions
{
    public class RedisPubSubMessage
    {
        public string Channel { get; set; }
        public string Message { get; set; }
    }
}
