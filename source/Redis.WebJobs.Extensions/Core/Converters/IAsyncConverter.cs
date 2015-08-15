using System.Threading;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Converters
{
    internal interface IAsyncConverter<TInput, TOutput>
    {
        Task<TOutput> ConvertAsync(TInput input, CancellationToken cancellationToken);
    }
}
