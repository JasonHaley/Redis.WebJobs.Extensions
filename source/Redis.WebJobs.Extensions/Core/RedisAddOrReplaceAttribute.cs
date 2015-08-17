using System;

namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RedisAddOrReplaceAttribute : Attribute
    {
        public RedisAddOrReplaceAttribute(string keyName)
        {
            KeyName = keyName;
        }

        public string KeyName { get; private set; }
    }
}
