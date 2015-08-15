using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisPubSubParameterDescriptor : ParameterDescriptor
    {
        public string ChannelName { get; set; }
    }
}
