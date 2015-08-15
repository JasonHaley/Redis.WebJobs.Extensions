
using Newtonsoft.Json;

namespace Redis.WebJobs.Extensions
{
    internal static class Constants
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.DateTimeOffset,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
    }
}
