using System;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class StringToRedisPubSubMessageConverterFactory
    {
        public static IConverter<string, string> Create(Type parameterType)
        {
            if (parameterType == typeof(string))
            {
                return new StringToTextRedisPubSubMessageConverter();
            }
            else
            {
                return new StringToJsonRedisPubSubMessageConverter();
            }
        }
    }
}

