using System.Collections.Generic;
using System.Globalization;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class RedisSubscribeTriggerParameterDescriptor : TriggerParameterDescriptor
    {
        public string ChannelName { get; set; }
        
        public override string GetTriggerReason(IDictionary<string, string> arguments)
        {
            return string.Format(CultureInfo.CurrentCulture, "New message detected on '{0}'.", ChannelName);
        }
    }
}
