using System;

namespace Redis.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RedisGetAttribute : Attribute
    {
        public RedisGetAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}
