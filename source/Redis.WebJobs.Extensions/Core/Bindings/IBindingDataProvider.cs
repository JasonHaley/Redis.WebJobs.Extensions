using System;
using System.Collections.Generic;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal interface IBindingDataProvider
    {
        IReadOnlyDictionary<string, Type> Contract { get; }

        IReadOnlyDictionary<string, object> GetBindingData(object value);
    }
}
