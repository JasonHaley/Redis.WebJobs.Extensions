using System.Diagnostics.CodeAnalysis;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class StringToJsonRedisPubSubMessageConverter : IConverter<string, string>
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public string Convert(string input)
        {
            return input;
        }
    }
}
