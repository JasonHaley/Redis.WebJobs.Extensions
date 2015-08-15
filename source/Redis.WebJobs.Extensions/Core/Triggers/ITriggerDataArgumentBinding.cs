using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal interface ITriggerDataArgumentBinding<TTriggerValue>
    {
        Type ValueType { get; }

        IReadOnlyDictionary<string, Type> BindingDataContract { get; }

        Task<ITriggerData> BindAsync(TTriggerValue value, ValueBindingContext context);
    }
}
