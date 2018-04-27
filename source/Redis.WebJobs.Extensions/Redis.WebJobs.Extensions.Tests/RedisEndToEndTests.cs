using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Redis.WebJobs.Extensions;
using Redis.WebJobs.Extensions.Tests.Common;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Redis.WebJobs.Extensions.Tests
{
    public class RedisEndToEndTests
    {
        [Fact]
        public async Task OutputBinding_PubSubString()
        {
            // Arrange
            string channelName = "pubsub:Channel9";
            string message = "String message";
            
            string testName = nameof(RedisEndToEndFunctions.SendSimplePubSubMessage);
            Type testType = typeof(RedisEndToEndTests);
            ExplicitTypeLocator locator = new ExplicitTypeLocator(testType);

            // Act
            JobHostConfiguration config = new JobHostConfiguration
            {
                TypeLocator = locator
            };

            config.UseRedis();

            JobHost host = new JobHost(config);

            await host.StartAsync();
            await host.CallAsync(testType.GetMethod(testName));
            await host.StopAsync();

            //// Assert
            //Assert.Equal(channelName, attribute.ChannelOrKey);
            //Assert.Equal(Mode.PubSub, attribute.Mode);
        }

        private class RedisEndToEndFunctions
        {
            public static void SendSimplePubSubMessage(
                [Redis("pubsub:stringMessages")] out string message)
            {
                message = "This is a test";
            }
        }
    }
}
