
using Xunit;

namespace Redis.WebJobs.Extensions.Tests
{
    public class RedisAttributeTests
    {
        [Fact]
        public void Constructor_ChannelName()
        {
            // Arrange
            string channelName = "Channel9";

            // Act
            RedisAttribute attribute = new RedisAttribute(channelName);

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
            RedisAttribute attribute = new RedisAttribute(channelName, mode);

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
            RedisAttribute attribute = new RedisAttribute(channelName, mode, connectionString);

            // Assert
            Assert.Equal(channelName, attribute.ChannelOrKey);
            Assert.Equal(mode, attribute.Mode);
            Assert.Equal(connectionString, attribute.ConnectionStringSetting);
        }
    }
}
