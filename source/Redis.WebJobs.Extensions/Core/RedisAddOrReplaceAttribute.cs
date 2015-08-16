using System;

namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RedisAddOrReplaceAttribute : Attribute
    {
        public RedisAddOrReplaceAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}
