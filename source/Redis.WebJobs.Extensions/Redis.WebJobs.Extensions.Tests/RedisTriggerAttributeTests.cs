using Xunit;

namespace Redis.WebJobs.Extensions.Tests
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
            Assert.Null(attribute.ConnectionStringSetting);
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
            Assert.Null(attribute.ConnectionStringSetting);
        }

        [Fact]
        public void Constructor_ChannelModeAndConnectionString()
        {
            // Arrange
            string channelName = "Channel9";
            Mode mode = Mode.Cache;
            string connectionString = "connection";

            // Act
            RedisTriggerAttribute attribute = new RedisTriggerAttribute(channelName, mode, connectionString);

            // Assert
            Assert.Equal(channelName, attribute.ChannelOrKey);
            Assert.Equal(mode, attribute.Mode);
            Assert.Equal(connectionString, attribute.ConnectionStringSetting);
        }
    }
}
