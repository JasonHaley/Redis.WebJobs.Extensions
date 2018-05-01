using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;

namespace Subscriber
{
    public class NameResolver : INameResolver
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public NameResolver()
        {
            _values.Add("CacheKey", "key");
        }

        public Dictionary<string, string> Values
        {
            get
            {
                return _values;
            }
        }

        public string Resolve(string name)
        {
            string value = null;

            value = Environment.GetEnvironmentVariable(name);

            if (value == null)
            {
                Values.TryGetValue(name, out value);
            }

            return value;
        }
    }
}
