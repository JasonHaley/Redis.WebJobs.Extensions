using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisPublishBinding : IBinding
    {
        private readonly string _parameterName;
        private readonly IArgumentBinding<RedisPubSubEntity> _argumentBinding;
        private readonly RedisAccount _account;
        private readonly string _channelName;
        
        public RedisPublishBinding(string parameterName, IArgumentBinding<RedisPubSubEntity> argumentBinding,
            RedisAccount account, string channelName)
        {
            _parameterName = parameterName;
            _argumentBinding = argumentBinding;
            _account = account;
            _channelName = channelName;
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

            return await BindAsync((RedisPubSubEntity)value, context);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RedisPubSubParameterDescriptor
            {
                Name = _parameterName,
                ChannelName = _channelName,
                DisplayHints = CreateParameterDisplayHints(_channelName, false)
            };
        }

        private Task<IValueProvider> BindAsync(RedisPubSubEntity value, ValueBindingContext context)
        {
            return _argumentBinding.BindAsync(value, context);
        }

        private RedisPubSubEntity CreateEntity()
        {
            return new RedisPubSubEntity
            {
                Account = _account,
                ChannelName = _channelName
            };
        }
        
        internal static ParameterDisplayHints CreateParameterDisplayHints(string channelName, bool isInput)
        {
            ParameterDisplayHints descriptor = new ParameterDisplayHints();

            descriptor.Description = isInput ?
                string.Format(CultureInfo.CurrentCulture, "publish to channel '{0}'", channelName) :
                string.Format(CultureInfo.CurrentCulture, "subscribe to channel '{0}'", channelName);

            descriptor.Prompt = isInput ?
                "Enter the message" :
                "Enter the channel name";

            descriptor.DefaultValue = isInput ? null : channelName;

            return descriptor;
        }

        private class RedisPubSubParameterDescriptor : ParameterDescriptor
        {
            public string ChannelName { get; set; }
        }
    }
}
