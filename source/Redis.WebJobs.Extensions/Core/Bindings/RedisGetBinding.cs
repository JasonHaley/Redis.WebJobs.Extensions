using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisGetBinding : IBinding
    {
        private readonly string _parameterName;
        private readonly IArgumentBinding<RedisKeyEntity> _argumentBinding;
        private readonly RedisAccount _account;
        private readonly string _keyName;
        public RedisGetBinding(string parameterName, IArgumentBinding<RedisKeyEntity> argumentBinding, RedisAccount account, string keyName)
        {
            _parameterName = parameterName;
            _argumentBinding = argumentBinding;
            _account = account;
            _keyName = keyName;
        }

        public bool FromAttribute
        {
            get { return true; }
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var entity = CreateEntity();

            return BindAsync(entity, context.ValueContext);
        }

        public async Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            if (value == null)
            {
                value = CreateEntity();
            }

            return await BindAsync((RedisKeyEntity)value, context);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RedisGetBinding.RedisKeyParameterDescriptor
            {
                Name = _parameterName,
                KeyName = _keyName,
                DisplayHints = CreateParameterDisplayHints(_keyName, false)
            };
        }

        private Task<IValueProvider> BindAsync(RedisKeyEntity value, ValueBindingContext context)
        {
            return _argumentBinding.BindAsync(value, context);
        }

        private RedisKeyEntity CreateEntity()
        {
            return new RedisKeyEntity
            {
                Account = _account,
                KeyName = _keyName
            };
        }

        internal static ParameterDisplayHints CreateParameterDisplayHints(string keyName, bool isInput)
        {
            ParameterDisplayHints descriptor = new ParameterDisplayHints();

            descriptor.Description = isInput ?
                string.Format(CultureInfo.CurrentCulture, "set redis key value '{0}'", keyName) :
                string.Format(CultureInfo.CurrentCulture, "get redis key value '{0}'", keyName);

            descriptor.Prompt = isInput ?
                "Enter the value" :
                "Enter the key";

            descriptor.DefaultValue = isInput ? null : keyName;

            return descriptor;
        }

        private class RedisKeyParameterDescriptor : ParameterDescriptor
        {
            public string KeyName { get; set; }
        }
    }
}
