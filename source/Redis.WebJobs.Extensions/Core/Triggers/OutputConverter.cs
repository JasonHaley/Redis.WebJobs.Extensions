using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class OutputConverter<TInput> : IObjectToTypeConverter<string>
       where TInput : class
    {
        private readonly IConverter<TInput, string> _innerConverter;

        public OutputConverter(IConverter<TInput, string> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public bool TryConvert(object input, out string output)
        {
            TInput typedInput = input as TInput;

            if (typedInput == null)
            {
                output = null;
                return false;
            }

            output = _innerConverter.Convert(typedInput);
            return true;
        }
    }
}

