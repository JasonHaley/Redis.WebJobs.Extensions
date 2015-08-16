using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json.Linq;
using Redis.WebJobs.Extensions.Framework;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisPubSubValueBinder<TInput> : JsonStringValueBinder<TInput>
    {
        private readonly RedisPubSubEntity _entity;

        public RedisPubSubValueBinder(RedisPubSubEntity entity) 
            : base(BindStepOrder.Enqueue)
        {
            _entity = entity;
        }

        public override object GetValue()
        {
            return default(TInput);
        }

        public override string ToInvokeString()
        {
            return _entity.ChannelName;
        }

        public override Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            string message;
            if (typeof (TInput) == typeof (string))
            {
                message = value.ToString();
            }
            else
            {
                message = ConvertToJson((TInput) value);
            }

            return _entity.SendAsync(message);
        }
        
    }
}
