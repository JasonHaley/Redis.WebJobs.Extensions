
namespace Redis.WebJobs.Extensions.Converters
{
    internal interface IObjectToTypeConverter<TOutput>
    {
        bool TryConvert(object input, out TOutput output);
    }
}
