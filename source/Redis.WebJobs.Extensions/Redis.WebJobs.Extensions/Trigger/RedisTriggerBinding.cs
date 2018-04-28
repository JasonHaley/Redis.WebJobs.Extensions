using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Redis.WebJobs.Extensions.Framework;
using Redis.WebJobs.Extensions.Listeners;

namespace Redis.WebJobs.Extensions.Trigger
{
    internal class RedisTriggerBinding : ITriggerBinding
    {
        private readonly RedisConfiguration _configuration;
        private readonly ParameterInfo _parameter;
        private readonly IRedisAttribute _attribute;
        private readonly BindingDataProvider _bindingDataProvider;
        private readonly IReadOnlyDictionary<string, Type> _emptyBindingContract = new Dictionary<string, Type>();
        private readonly IReadOnlyDictionary<string, object> _emptyBindingData = new Dictionary<string, object>();

        public RedisTriggerBinding(RedisConfiguration configuration, ParameterInfo parameter, IRedisAttribute attribute)
        {
            _configuration = configuration;
            _parameter = parameter;
            _attribute = attribute;
            //_bindingDataProvider = BindingDataProvider.FromTemplate(_attribute.ChannelOrKey); // ?? {Id} ??
            _bindingDataProvider = BindingDataProvider.FromType(parameter.ParameterType);
        }

        public string ChannelOrKey => _attribute.ChannelOrKey;
        public Mode Mode => _attribute.Mode;

        public Type TriggerValueType => typeof(string);

        public IReadOnlyDictionary<string, Type> BindingDataContract
        {
            get { return _bindingDataProvider?.Contract; }
        }

        public async Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            IValueProvider provider = new JsonValueProvider(value, _parameter.ParameterType);

            var providerVal = await provider.GetValueAsync();
            var bindingData = _bindingDataProvider?.GetBindingData(providerVal);

            var result = new TriggerData(provider, bindingData);

            return result;
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IListener listener;

            //if (_mode == Mode.PubSub)
            //{
                listener = new RedisChannelListener(_configuration, _attribute, context.Executor);
            //}
            //else
            //{
            //    listener = new RedisCacheListener(_channelOrKey, context.Executor, _config, _trace);
            //}
            return Task.FromResult(listener);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RedisTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                ChannelOrKey = _attribute.ChannelOrKey,
                Mode = _attribute.Mode,
                DisplayHints = RedisTriggerParameterDescriptor.CreateParameterDisplayHints(ChannelOrKey, Mode, true)
            };
        }

        private class RedisTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public string ChannelOrKey { get; set; }
            public Mode Mode { get; set; }

            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                return (Mode == Mode.PubSub) ? $"New message detected on '{ChannelOrKey}'." : $"New value found at '{ChannelOrKey}'";
            }

            internal static ParameterDisplayHints CreateParameterDisplayHints(string channelOrKey, Mode mode, bool isInput)
            {
                ParameterDisplayHints descriptor = new ParameterDisplayHints();

                if (mode == Mode.PubSub)
                {
                    descriptor.Description = isInput ?
                        $"publish to channel '{channelOrKey}'" :
                        $"subscribe to channel '{channelOrKey}'";
                }
                else
                {
                    descriptor.Description = isInput ?
                        $"key to get from cache '{channelOrKey}'" :
                        $"key to add to cache'{channelOrKey}'";
                }
                descriptor.Prompt = isInput ?
                    "Enter the channel name" :
                    "Enter the key name";

                descriptor.DefaultValue = isInput ? null : channelOrKey;

                return descriptor;
            }
        }
    }

}
