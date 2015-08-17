using System;

namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RedisGetAttribute : Attribute
    {
        public RedisGetAttribute(string keyName)
        {
            KeyName = keyName;
        }

        public string KeyName { get; private set; }
    }
}
