using System;

namespace Redis.WebJobs.Extensions.Converters
{
    internal class StringToRedisPubSubMessageConverter : IConverter<string, RedisPubSubMessage>
    {
        public RedisPubSubMessage Convert(string input)
        {
            if (input == null)
            {
                throw new InvalidOperationException("A message cannot contain a null string instance.");
            }

            return new RedisPubSubMessage
            {
                Message = input
            };
        }
    }
}
