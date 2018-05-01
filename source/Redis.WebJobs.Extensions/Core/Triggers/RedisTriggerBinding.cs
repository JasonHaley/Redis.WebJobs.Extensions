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
        private readonly RedisTriggerAttribute _attribute;
        private readonly ParameterInfo _parameter;
        private readonly BindingDataProvider _bindingDataProvider;
        private readonly IReadOnlyDictionary<string, Type> _bindingContract;
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
            //_bindingDataProvider = BindingDataProvider.FromType(parameter.ParameterType);
            _attribute = parameter.GetCustomAttribute<RedisTriggerAttribute>(inherit: false);
            _bindingDataProvider = BindingDataProvider.FromTemplate(_attribute.ChannelOrKey);
            //_bindingContract = CreateBindingContract();
            _trace = trace;
        }
        
        public Type TriggerValueType => typeof(string);

        public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingDataProvider?.Contract;

        public string ChannelOrKey => _channelOrKey;

        public Mode Mode => _mode;

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            IValueProvider provider = new JsonValueProvider(value, _parameter.ParameterType);

            var providerVal = provider.GetValue();
            var bindingData = _bindingDataProvider?.GetBindingData(providerVal);

            var result = new TriggerData(provider, bindingData);

            return Task.FromResult< ITriggerData>(result);
        }
        //private IReadOnlyDictionary<string, Type> CreateBindingContract()
        //{
        //    Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        //    //contract.Add("FileTrigger", typeof(FileSystemEventArgs));

        //    if (_bindingDataProvider.Contract != null)
        //    {
        //        foreach (KeyValuePair<string, Type> item in _bindingDataProvider.Contract)
        //        {
        //            // In case of conflict, binding data from the value type overrides the built-in binding data above.
        //            contract[item.Key] = item.Value;
        //        }
        //    }

        //    return contract;
        //}

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