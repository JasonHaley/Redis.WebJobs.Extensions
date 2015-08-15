using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class NonNullConverterValueBinder<TInput> : IOrderedValueBinder
    {
        private readonly RedisPubSubEntity _entity;
        private readonly IConverter<TInput, RedisPubSubMessage> _converter;

        public NonNullConverterValueBinder(RedisPubSubEntity entity, IConverter<TInput, RedisPubSubMessage> converter)
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
            return null;
        }
        public string ToInvokeString()
        {
            return _entity.ChannelName;
        }

        public Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            if (value == null)
            {
                return Task.FromResult(0);
            }
            RedisPubSubMessage message = _converter.Convert((TInput)value);

            return _entity.SendAsync(message.Message);
        }


    }
}
