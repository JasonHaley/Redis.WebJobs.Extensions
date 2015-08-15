
namespace Redis.WebJobs.Extensions.Converters
{
    internal struct ConversionResult<TResult>
    {
        public bool Succeeded { get; set; }

        public TResult Result { get; set; }
    }
}
