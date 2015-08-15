
namespace Redis.WebJobs.Extensions.Converters
{
    internal class IdentityConverter<TValue> : IConverter<TValue, TValue>
    {
        public TValue Convert(TValue input)
        {
            return input;
        }
    }
}