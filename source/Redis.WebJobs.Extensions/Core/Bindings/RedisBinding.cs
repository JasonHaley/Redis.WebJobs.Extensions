using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisBinding : IBinding
    {
        private readonly string _parameterName;
        private readonly IArgumentBinding<RedisEntity> _argumentBinding;
        private readonly RedisAccount _account;
        private readonly string _channelOrKey;
        private readonly Mode _mode;
        
        public RedisBinding(string parameterName, IArgumentBinding<RedisEntity> argumentBinding,
            RedisAccount account, string channelOrKey, Mode mode)
        {
            _parameterName = parameterName;
            _argumentBinding = argumentBinding;
            _account = account;
            _channelOrKey = channelOrKey;
            _mode = mode;
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

            return await BindAsync((RedisEntity)value, context);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RedisParameterDescriptor
            {
                Name = _parameterName,
                ChannelOrKey = _channelOrKey,
                Mode = _mode,
                DisplayHints = CreateParameterDisplayHints(_channelOrKey, _mode, false)
            };
        }

        private Task<IValueProvider> BindAsync(RedisEntity value, ValueBindingContext context)
        {
            return _argumentBinding.BindAsync(value, context);
        }

        private RedisEntity CreateEntity()
        {
            return new RedisEntity
            {
                Account = _account,
                ChannelOrKey = _channelOrKey,
                Mode = _mode
            };
        }
        
        internal static ParameterDisplayHints CreateParameterDisplayHints(string channelOrKey, Mode mode, bool isInput)
        {
            ParameterDisplayHints descriptor = new ParameterDisplayHints();

            if (mode == Mode.PubSub)
            {
                descriptor.Description = isInput
                    ? string.Format(CultureInfo.CurrentCulture, "publish to channel '{0}'", channelOrKey)
                    : string.Format(CultureInfo.CurrentCulture, "subscribe to channel '{0}'", channelOrKey);
            }
            else
            {
                descriptor.Description = isInput
                    ? string.Format(CultureInfo.CurrentCulture, "key to get from cache '{0}'", channelOrKey)
                    : string.Format(CultureInfo.CurrentCulture, "key to add to cache'{0}'", channelOrKey);
            }
            descriptor.Prompt = isInput ?
                "Enter the channel name" :
                "Enter the key name";

            descriptor.DefaultValue = isInput ? null : channelOrKey;

            return descriptor;
        }

        private class RedisParameterDescriptor : ParameterDescriptor
        {
            public string ChannelOrKey { get; set; }
            public Mode Mode { get; set; }
        }
    }
}
