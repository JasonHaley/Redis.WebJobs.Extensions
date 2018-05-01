using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using Moq;
using Redis.WebJobs.Extensions.Services;
using Redis.WebJobs.Extensions.Tests.Common;
using System;
using Xunit;

namespace Redis.WebJobs.Extensions.Tests.Config
{
    public class RedisConfigurationTests
    {
        [Fact]
        public void Constructor_SetsExpectedDefaults()
        {
            // Arrange            
            var config = new RedisConfiguration();

            // Act

            // Assert
            Assert.Null(config.ConnectionString);
            Assert.Equal(TimeSpan.FromSeconds(30), config.CheckCacheFrequency);
            Assert.Equal("Previous_", config.LastValueKeyNamePrefix);
        }

        [Fact]
        public void Constructor_ConnectionString()
        {
            // Arrange
            var config = new RedisConfiguration("RedisConnectionString");

            // Act
            string defaultConnection = AmbientConnectionStringProvider.Instance.GetConnectionString("RedisConnectionString");

            // Assert
            Assert.Equal(defaultConnection, config.ConnectionString);
        }

        [Fact]
        public void ResolveConnectionString_UsesAttribute_First()
        {
            // Arrange            
            var config = InitializeConfig("Default");
            config.ConnectionString = "Config";

            // Act
            var connString = config.ResolveConnectionString("Attribute");

            // Assert
            Assert.Equal("Attribute", connString);
        }

        [Fact]
        public void ResolveConnectionString_UsesConfig_Second()
        {
            // Arrange            
            var config = InitializeConfig("Default");
            config.ConnectionString = "Config";

            // Act
            var connString = config.ResolveConnectionString(null);

            // Assert
            Assert.Equal("Config", connString);
        }

        [Fact]
        public void CreateService_UsesServiceFactory()
        {
            // Arange
            var serviceMock = new Mock<IRedisService>(MockBehavior.Strict);
            
            var factoryMock = new Mock<IRedisServiceFactory>(MockBehavior.Strict);
            factoryMock.Setup(m => m.CreateService(It.IsAny<string>()))
                .Returns<string>((connectionString) => 
                {
                    return serviceMock.Object;
                });

            var config = InitializeConfig("Default");
            config.RedisServiceFactory = factoryMock.Object;

            var attribute = new RedisAttribute("channel");

            // Act
            var service = config.CreateService(attribute);

            // Assert
            factoryMock.Verify(f => f.CreateService("Default"), Times.Once());

        }

        private RedisConfiguration InitializeConfig(string connectionStringSetting)
        {
            var config = new RedisConfiguration();

            var nameResolver = new TestNameResolver();
            nameResolver.Values[RedisConfiguration.AzureWebJobsRedisConnectionStringSetting] = connectionStringSetting;

            var jobHostConfig = new JobHostConfiguration();
            jobHostConfig.AddService<INameResolver>(nameResolver);

            var context = new ExtensionConfigContext()
            {
                Config = jobHostConfig
            };

            config.Initialize(context);

            return config;
        }
    }
}

// Arrange
// Act
// Assert
