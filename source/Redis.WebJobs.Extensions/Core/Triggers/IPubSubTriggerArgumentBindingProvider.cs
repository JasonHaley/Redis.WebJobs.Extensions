using System.Reflection;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal interface IPubSubTriggerArgumentBindingProvider
    {
        ITriggerDataArgumentBinding<string> TryCreate(ParameterInfo parameter);
    }
}
