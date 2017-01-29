using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Redis.WebJobs.Extensions.Bindings;
using Redis.WebJobs.Extensions.Config;
using Redis.WebJobs.Extensions.Framework;
using Redis.WebJobs.Extensions.Listeners;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class RedisTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly IBindingDataProvider _bindingDataProvider;
        private readonly string _channelOrKey;
        private readonly Mode _mode;
        private readonly RedisConfiguration _config;
        private readonly TraceWriter _trace;

        public RedisTriggerBinding(ParameterInfo parameter, string channelOrKey, Mode mode, RedisConfiguration config, TraceWriter trace)
        {
            _parameter = parameter;
            _channelOrKey = channelOrKey;
            _mode = mode;
            _config = config;
            _bindingDataProvider = BindingDataProvider.FromType(parameter.ParameterType);
            _trace = trace;
        }
        
        public Type TriggerValueType => typeof(string);

        public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingDataProvider?.Contract;

        public string ChannelOrKey => _channelOrKey;

        public Mode Mode => _mode;

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
            
            if (_mode == Mode.PubSub)
            {
                listener = new RedisChannelListener(_channelOrKey, context.Executor, _config, _trace);
            }
            else
            {
                listener = new RedisCacheListener(_channelOrKey, context.Executor, _config, _trace);
            }
            return Task.FromResult(listener);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RedisTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                ChannelOrKey = _channelOrKey,
                Mode = _mode,
                DisplayHints = RedisBinding.CreateParameterDisplayHints(ChannelOrKey, Mode, true)
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
        }
    }
}