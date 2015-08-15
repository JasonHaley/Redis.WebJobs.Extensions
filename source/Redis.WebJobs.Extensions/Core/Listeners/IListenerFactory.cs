using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Redis.WebJobs.Extensions.Listeners
{
    internal interface IListenerFactory
    {
        Task<IListener> CreateAsync(CancellationToken token);
    }
}
