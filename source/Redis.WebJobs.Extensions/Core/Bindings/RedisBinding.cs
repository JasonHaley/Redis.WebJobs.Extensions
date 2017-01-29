using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings.Path;
namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisBinding : IBinding
    {
        private readonly string _parameterName;
        private readonly IArgumentBinding<RedisEntity> _argumentBinding;
        private readonly RedisAccount _account;
        private readonly RedisAttribute _attribute;
        private readonly BindingTemplate _channelOrKeyPath;
        private readonly Mode _mode;
        private readonly TraceWriter _trace;

        public RedisBinding(string parameterName, IArgumentBinding<RedisEntity> argumentBinding,
            RedisAccount account, RedisAttribute attribute, BindingProviderContext context, TraceWriter trace)
        {
            _parameterName = parameterName;
            _argumentBinding = argumentBinding;
            _account = account;
            _attribute = attribute;
            _mode = attribute.Mode;
            
            _channelOrKeyPath = BindingTemplate.FromString(attribute.ChannelOrKey);
            _trace = trace;
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            
            var entity = CreateEntity(context.BindingData);

            return BindAsync(entity, context.ValueContext);
        }

        public async Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            if (value == null)
            {
                System.Diagnostics.Debugger.Break();
                //value = CreateEntity();
            }

            return await BindAsync((RedisEntity)value, context);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RedisParameterDescriptor
            {
                Name = _parameterName,
                ChannelOrKey = _attribute.ChannelOrKey,
                Mode = _mode,
                DisplayHints = CreateParameterDisplayHints(_attribute.ChannelOrKey, _mode, false)
            };
        }

        private Task<IValueProvider> BindAsync(RedisEntity value, ValueBindingContext context)
        {
            return _argumentBinding.BindAsync(value, context);
        }

        private RedisEntity CreateEntity(IReadOnlyDictionary<string, object> bindingData)
        {
            return new RedisEntity(_account, _channelOrKeyPath, _mode, bindingData);
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

        private class RedisParameterDescriptor : ParameterDescriptor
        {
            public string ChannelOrKey { get; set; }
            public Mode Mode { get; set; }
        }
    }
}
