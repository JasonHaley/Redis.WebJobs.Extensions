using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;

namespace Redis.WebJobs.Extensions.Framework
{
    internal abstract class JsonStringValueBinder<TInput> : IOrderedValueBinder
    {
        private readonly BindStepOrder _bindStepOrder;

        public JsonStringValueBinder(BindStepOrder bindStepOrder)
        {
            _bindStepOrder = bindStepOrder;
        }

        public BindStepOrder StepOrder
        {
            get { return _bindStepOrder; }
        }

        public Type Type
        {
            get { return typeof(TInput); }
        }

        public abstract object GetValue();

        /// <inheritdoc/>
        public abstract string ToInvokeString();

        public virtual Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        protected string ConvertToJson(TInput input)
        {
            return JsonConvert.SerializeObject(input, Constants.JsonSerializerSettings);
        }
        
        protected bool TryJsonConvert(string message, out TInput contents)
        {
            contents = default(TInput);
            try
            {
                contents = JsonConvert.DeserializeObject<TInput>(message, Constants.JsonSerializerSettings);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
