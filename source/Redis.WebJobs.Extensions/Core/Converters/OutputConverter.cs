using System.Threading;
using System.Threading.Tasks;
using Redis.WebJobs.Extensions.Bindings;

namespace Redis.WebJobs.Extensions.Converters
{
    internal class OutputConverter<TInput> : IAsyncObjectToTypeConverter<RedisPubSubEntity>
        where TInput : class
    {
        private readonly IAsyncConverter<TInput, RedisPubSubEntity> _innerConverter;

        public OutputConverter(IAsyncConverter<TInput, RedisPubSubEntity> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public async Task<ConversionResult<RedisPubSubEntity>> TryConvertAsync(object input,
            CancellationToken cancellationToken)
        {
            TInput typedInput = input as TInput;

            if (typedInput == null)
            {
                return new ConversionResult<RedisPubSubEntity>
                {
                    Succeeded = false,
                    Result = null
                };
            }

            RedisPubSubEntity entity = await _innerConverter.ConvertAsync(typedInput, cancellationToken);

            return new ConversionResult<RedisPubSubEntity>
            {
                Succeeded = true,
                Result = entity
            };
        }
    }
}

