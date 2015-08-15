using System.Threading;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Converters
{
    internal interface IAsyncObjectToTypeConverter<TOutput>
    {
        Task<ConversionResult<TOutput>> TryConvertAsync(object input, CancellationToken cancellationToken);
    }
}
