using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using Redis.WebJobs.Extensions.Bindings;
using Redis.WebJobs.Extensions.Framework;
using Redis.WebJobs.Extensions.Services;
using Redis.WebJobs.Extensions.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions
{
    public class RedisConfiguration : IExtensionConfigProvider
    {
        internal const string AzureWebJobsRedisConnectionStringSetting = "AzureWebJobsRedisConnectionString";
        
        public RedisConfiguration(string connectionStringSetting)
           : this()
        {
            SetConnectionString(connectionStringSetting);
        }

        public RedisConfiguration()
        {
            LastValueKeyNamePrefix = "Previous_";
            CheckCacheFrequency = TimeSpan.FromSeconds(30);

            RedisServiceFactory = new DefaultRedisServiceFactory();
        }

        internal IRedisServiceFactory RedisServiceFactory { get; set; }

        public string ConnectionString { get; set; }
        public TimeSpan? CheckCacheFrequency { get; set; }
        public string LastValueKeyNamePrefix { get; set; }

        private void SetConnectionString(string settingName)
        {
            ConnectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(settingName);
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            INameResolver nameResolver = context.Config.GetService<INameResolver>();

            if (string.IsNullOrEmpty(ConnectionString))
            {
                var resolvedConnectionStringSetting = nameResolver.Resolve(AzureWebJobsRedisConnectionStringSetting);
                ConnectionString = resolvedConnectionStringSetting;
            }

                        
            var bindingRule = context.AddBindingRule<RedisAttribute>();
            bindingRule.AddValidator(ValidateConnection);
            //bindingRule.BindToCollector<string>(CreateCollector);
            bindingRule.BindToCollector<RedisMessageOpenType>(typeof(RedisOpenTypeConverter<>), this);

            var triggerRule = context.AddBindingRule<RedisTriggerAttribute>();
            triggerRule.BindToTrigger<string>(new RedisTriggerAttributeBindingProvider(this));
            //triggerRule.AddConverter<string, IReadOnlyList<string>>(str => JsonConvert.DeserializeObject<IReadOnlyList<string>>(str));
            //triggerRule.AddConverter<IReadOnlyList<string>, string>(docList => JArray.FromObject(docList).ToString());
            triggerRule.AddOpenConverter<string, RedisMessageOpenType>(typeof(RedisMessageOpenTypeBindingConverter<>));
            
            //triggerRule.BindToValueProvider<RedisMessageOpenType>((attr, t) => BindForItemAsync(attr, t));
            //triggerRule.AddConverter<IReadOnlyList<string>, JArray>(docList => JArray.FromObject(docList));


        }
        private IAsyncCollector<string> CreateCollector(RedisAttribute attribute)
        {
            IRedisService service = CreateService(attribute);
            return new RedisMessageAsyncCollector<string>(this, attribute, service);
        }

        internal Task<IValueBinder> BindForItemAsync(IRedisAttribute attribute, Type type)
        {
            if (string.IsNullOrEmpty(attribute.ChannelOrKey))
            {
                throw new InvalidOperationException("The 'ChannelOrKey' property of a CosmosDB single-item input binding cannot be null or empty.");
            }

            RedisContext context = CreateContext(attribute);

            Type genericType = typeof(RedisValueBinder<>).MakeGenericType(type);
            IValueBinder binder = (IValueBinder)Activator.CreateInstance(genericType, context);

            return Task.FromResult(binder);
        }
        internal void ValidateConnection(RedisAttribute attribute, Type paramType)
        {
            if (string.IsNullOrEmpty(ConnectionString) &&
                string.IsNullOrEmpty(attribute.ConnectionStringSetting))
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                    "The Redis connection string must be set either via a '{0}' app setting, via the RedisAttribute.ConnectionStringSetting property or via RedisConfiguration.ConnectionString.",
                    AzureWebJobsRedisConnectionStringSetting));
            }
        }
        internal string ResolveConnectionString(string attributeConnectionString)
        {
            // First, try the Attribute's string.
            if (!string.IsNullOrEmpty(attributeConnectionString))
            {
                return attributeConnectionString;
            }

            // Second, try the config's ConnectionString
            return ConnectionString;
        }
        internal IRedisService CreateService(IRedisAttribute attribute)
        {
            string resolvedConnectionString = ResolveConnectionString(attribute.ConnectionStringSetting);
            return RedisServiceFactory.CreateService(resolvedConnectionString);
        }

        internal RedisContext CreateContext(IRedisAttribute attribute)
        {
            IRedisService service = CreateService(attribute);

            return new RedisContext
            {
                ResolvedAttribute = attribute,
                Service = service
            };
        }

        private class RedisMessageOpenType : OpenType.Poco
        {
            public override bool IsMatch(Type type, OpenTypeMatchContext context)
            {
                if (type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return false;
                }

                if (type.FullName == "System.Object")
                {
                    return true;
                }

                return base.IsMatch(type, context);
            }
        }

        private class RedisMessageOpenTypeBindingConverter<TOutput>
             : IConverter<string, TOutput>
        {
            public TOutput Convert(string input)
            {
                if (input != null)
                {
                    return ConvertFromJson<TOutput>(input);
                }
                return default(TOutput);
            }

            private TOutput ConvertFromJson<TOutput>(string json)
            {
                return JsonConvert.DeserializeObject<TOutput>(json);
            }
        }
    }
}
