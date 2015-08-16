using System;
using System.Collections.Generic;

namespace Redis.WebJobs.Extensions.Framework
{
    internal class BindingDataProvider : IBindingDataProvider
    {
        private readonly Type _type;
        private readonly IReadOnlyDictionary<string, Type> _contract;
        private readonly IEnumerable<PropertyHelper> _propertyHelpers;

        internal BindingDataProvider(Type type, IReadOnlyDictionary<string, Type> contract, IEnumerable<PropertyHelper> propertyHelpers)
        {
            _type = type;
            _contract = contract;
            _propertyHelpers = propertyHelpers;
        }

        internal Type ValueType
        {
            get { return _type; }
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, Type> Contract
        {
            get { return _contract; }
        }

        public IReadOnlyDictionary<string, object> GetBindingData(object value)
        {
            if (value != null && value.GetType() != ValueType)
            {
                throw new ArgumentException("Provided value is not of the given type", "value");
            }

            if (Contract == null || value == null)
            {
                return null;
            }

            Dictionary<string, object> bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var propertyHelper in _propertyHelpers)
            {
                object propertyValue = propertyHelper.GetValue(value);
                bindingData.Add(propertyHelper.Name, propertyValue);
            }

            return bindingData;
        }

        public static BindingDataProvider FromType(Type type)
        {
            if (type == typeof(string))
            {
                return null;
            }

            IReadOnlyList<PropertyHelper> bindingDataProperties = PropertyHelper.GetProperties(type);

            if (bindingDataProperties.Count == 0)
            {
                return null;
            }

            Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyHelper property in bindingDataProperties)
            {
                contract.Add(property.Name, property.PropertyType);
            }

            return new BindingDataProvider(type, contract, bindingDataProperties);
        }
    }
}
