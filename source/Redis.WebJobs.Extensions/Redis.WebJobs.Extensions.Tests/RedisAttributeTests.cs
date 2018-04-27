
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
        }
    }
}
