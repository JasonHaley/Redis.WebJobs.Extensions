using Newtonsoft.Json;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class UserTypeToStringConverter<TInput> : IConverter<TInput, string>
    {
        public string Convert(TInput input)
        {
            return JsonConvert.SerializeObject(input, Constants.JsonSerializerSettings);
        }
    }
}
