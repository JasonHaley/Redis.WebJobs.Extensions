using System;
using System.Collections.Generic;

namespace Redis.WebJobs.Extensions.Converters
{
    internal class CompositeObjectToTypeConverter<T> : IObjectToTypeConverter<T>
    {
        private readonly IEnumerable<IObjectToTypeConverter<T>> _converters;

        public CompositeObjectToTypeConverter(IEnumerable<IObjectToTypeConverter<T>> converters)
        {
            if (converters == null)
            {
                throw new ArgumentNullException("converters");
            }
            _converters = converters;
        }

        public CompositeObjectToTypeConverter(params IObjectToTypeConverter<T>[] converters)
            : this((IEnumerable<IObjectToTypeConverter<T>>)converters)
        {
        }

        public bool TryConvert(object input, out T output)
        {
            foreach (IObjectToTypeConverter<T> converter in _converters)
            {
                T possibleConverted;

                if (converter.TryConvert(input, out possibleConverted))
                {
                    output = possibleConverted;
                    return true;
                }
            }

            output = default(T);
            return false;
        }
    }
}

