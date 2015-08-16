using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Redis.WebJobs.Extensions.Bindings;
using Redis.WebJobs.Extensions.Config;
using Redis.WebJobs.Extensions.Listeners;

namespace Redis.WebJobs.Extensions.Triggers
{
    internal class RedisSubscribeTriggerBinding : ITriggerBinding
    {
        private readonly string _parameterName;
        private readonly ITriggerDataArgumentBinding<string> _argumentBinding;
        private readonly RedisAccount _account;
        private readonly string _channelName;
        private readonly RedisConfiguration _config;

        public RedisSubscribeTriggerBinding(string parameterName, Type parameterType,
            ITriggerDataArgumentBinding<string> argumentBinding, RedisAccount account, string channelName, RedisConfiguration config)
        {
            _parameterName = parameterName;
            _argumentBinding = argumentBinding;
            _account = account;
            _channelName = channelName;
            _config = config;
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
            get { return _argumentBinding.BindingDataContract; }
        }

        public string ChannelName
        {
            get { return _channelName; }
        }
        
        public async Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            string message = value as string;
            if (message == null)
            {
                throw new InvalidOperationException("Unable to convert message.");
            }

            return await _argumentBinding.BindAsync(message, context);
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            IListenerFactory factory = null;
            factory = new RedisChannelListenerFactory(_account, _channelName, context.Executor, _config);
                        
            return factory.CreateAsync(context.CancellationToken);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RedisSubscribeTriggerParameterDescriptor
            {
                Name = _parameterName,
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