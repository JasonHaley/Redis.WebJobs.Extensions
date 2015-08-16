using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Redis.WebJobs.Extensions.Converters;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisPublishBinding : IBinding
    {
        private readonly string _parameterName;
        private readonly IArgumentBinding<RedisPubSubEntity> _argumentBinding;
        private readonly RedisAccount _account;
        private readonly string _channelName;
        private readonly IAsyncObjectToTypeConverter<RedisPubSubEntity> _converter;
        public RedisPublishBinding(string parameterName, IArgumentBinding<RedisPubSubEntity> argumentBinding,
            RedisAccount account, string channelName)
        {
            _parameterName = parameterName;
            _argumentBinding = argumentBinding;
            _account = account;
            _channelName = channelName;
            _converter = CreateConverter(account, channelName);
        }

        public bool FromAttribute
        {
            get { return true; }
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            RedisPubSubEntity entity = new RedisPubSubEntity
            {
                Account = _account,
                ChannelName = _channelName
            };

            return BindAsync(entity, context.ValueContext);
        }

        public async Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            ConversionResult<RedisPubSubEntity> conversionResult = await _converter.TryConvertAsync(value, context.CancellationToken);

            if (!conversionResult.Succeeded)
            {
                throw new InvalidOperationException("Unable to convert value to RedisPubSubEntity.");
            }

            return await BindAsync(conversionResult.Result, context);
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

        private static IAsyncObjectToTypeConverter<RedisPubSubEntity> CreateConverter(RedisAccount account, string channel)
        {
            return new OutputConverter<string>(new StringToRedisPubSubEntityConverter(account, channel));
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
