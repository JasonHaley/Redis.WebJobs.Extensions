using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
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
        private readonly RedisAccount _account;
        private readonly string _channelOrKey;
        private readonly Mode _mode;
        private readonly RedisConfiguration _config;

        public RedisTriggerBinding(ParameterInfo parameter, RedisAccount account, string channelOrKey, Mode mode, RedisConfiguration config)
        {
            _parameter = parameter;
            _account = account;
            _channelOrKey = channelOrKey;
            _mode = mode;
            _config = config;
            _bindingDataProvider = BindingDataProvider.FromType(parameter.ParameterType);
        }
        
        public Type TriggerValueType
        {
            get
            {
                return typeof(string);
            }
        }

        public IReadOnlyDictionary<string, Type> BindingDataContract
        {
            get { return _bindingDataProvider != null ? _bindingDataProvider.Contract : null; }
        }

        public string ChannelOrKey
        {
            get { return _channelOrKey; }
        }
        public Mode Mode
        {
            get { return _mode; }
        }

        public async Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            IValueProvider provider = new JsonValueProvider(value, _parameter.ParameterType);

            IReadOnlyDictionary<string, object> bindingData = (_bindingDataProvider != null)
                ? _bindingDataProvider.GetBindingData(provider.GetValue()) : null;

            return new TriggerData(provider, bindingData);
        }
        
        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            
            IListener listener;
            
            if (_mode == Mode.PubSub)
            {
                listener = new RedisChannelListener(_channelOrKey, context.Executor, _config);
            }
            else
            {
                listener = new RedisCacheListener(_channelOrKey, context.Executor, _config);
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
                return string.Format(CultureInfo.CurrentCulture, 
                    (Mode == Mode.PubSub) ? "New message detected on '{0}'." : "New value found at '{0}'", ChannelOrKey);
            }
        }
    }
}