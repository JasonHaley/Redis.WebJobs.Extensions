using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Redis.WebJobs.Extensions.Framework;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisAddOrReplaceValueBinder<TInput> : JsonStringValueBinder<TInput>
    {
        private readonly RedisKeyEntity _entity;

        public RedisAddOrReplaceValueBinder(RedisKeyEntity entity) 
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
            return _entity.KeyName;
        }

        public override Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            string valueToSet;
            if (typeof(TInput) == typeof(string))
            {
                valueToSet = value.ToString();
            }
            else
            {
                valueToSet = ConvertToJson((TInput)value);
            }

            return _entity.AddOrReplaceAsync(valueToSet);
        }

    }
}
