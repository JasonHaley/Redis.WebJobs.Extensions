using Xunit;

namespace Redis.WebJobs.Extensions.Tests.Trigger
{
    public class RedisTriggerAttributeTests
    {
        [Fact]
        public void Constructor_ChannelName()
        {
            // Arrange
            string channelName = "Channel9";

            // Act
            RedisTriggerAttribute attribute = new RedisTriggerAttribute(channelName);

            // Assert
            Assert.Equal(channelName, attribute.ChannelOrKey);
            Assert.Equal(Mode.PubSub, attribute.Mode);
        }

        [Fact]
        public void Constructor_ChannelAndMode()
        {
            // Arrange
            string channelName = "Channel9";
            Mode mode = Mode.Cache;

            // Act
            RedisTriggerAttribute attribute = new RedisTriggerAttribute(channelName, mode);

            // Assert
            Assert.Equal(channelName, attribute.ChannelOrKey);
            Assert.Equal(mode, attribute.Mode);
        }
    }
}
