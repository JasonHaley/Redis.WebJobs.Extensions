using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal interface IPubSubArgumentBindingProvider
    {
        IArgumentBinding<RedisPubSubEntity> TryCreate(ParameterInfo parameter);
    }
}
