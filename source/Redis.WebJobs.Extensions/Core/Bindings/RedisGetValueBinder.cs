using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Framework;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisGetValueBinder<TInput> : JsonStringValueBinder<TInput>
    {
        private readonly RedisKeyEntity _entity;

        public RedisGetValueBinder(RedisKeyEntity entity) 
            : base(BindStepOrder.Enqueue)
        {
            _entity = entity;
        }

        public override object GetValue()
        {
            string value = _entity.GetAsync().Result;

            TInput contents;
            if (TryJsonConvert(value, out contents))
            {
                return contents;
            }
            else
            {
                return value;
            }
        }

        public override string ToInvokeString()
        {
            return _entity.KeyName;
        }

        public override Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

    }
}
