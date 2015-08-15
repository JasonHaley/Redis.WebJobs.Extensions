
namespace Redis.WebJobs.Extensions.Converters
{
    internal interface IConverter<TInput, TOutput>
    {
        TOutput Convert(TInput input);
    }
}
