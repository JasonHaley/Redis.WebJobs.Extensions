using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class ConverterValueBinder<TInput> : IOrderedValueBinder
    {
        private readonly IConverter<TInput, string> _converter;
        private readonly RedisPubSubEntity _entity;

        public ConverterValueBinder(RedisPubSubEntity entity, IConverter<TInput, string> converter)
        {
            _entity = entity;
            _converter = converter;
        }

        public BindStepOrder StepOrder
        {
            get { return BindStepOrder.Enqueue; }
        }

        public Type Type
        {
            get { return typeof(TInput); }
        }

        public object GetValue()
        {
            return default(TInput);
        }
        public string ToInvokeString()
        {
            return _entity.ChannelName;
        }
        public Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            string message = _converter.Convert((TInput)value);
            Debug.Assert(message != null);

            return _entity.SendAsync(message);
        }

    }
}
