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
    internal class RedisSubscribeTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly IBindingDataProvider _bindingDataProvider;
        private readonly RedisAccount _account;
        private readonly string _channelName;
        private readonly RedisConfiguration _config;

        public RedisSubscribeTriggerBinding(ParameterInfo parameter, RedisAccount account, string channelName, RedisConfiguration config)
        {
            _parameter = parameter;
            _account = account;
            _channelName = channelName;
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

        public string ChannelName
        {
            get { return _channelName; }
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
            
            IListener listener = new RedisChannelListener(_account, _channelName, context.Executor, _config);
            return Task.FromResult(listener);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RedisSubscribeTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                ChannelName = _channelName,
                DisplayHints = RedisPublishBinding.CreateParameterDisplayHints(ChannelName, true)
            };
        }
        
        private class RedisSubscribeTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public string ChannelName { get; set; }

            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                return string.Format(CultureInfo.CurrentCulture, "New message detected on '{0}'.", ChannelName);
            }
        }
    }
}