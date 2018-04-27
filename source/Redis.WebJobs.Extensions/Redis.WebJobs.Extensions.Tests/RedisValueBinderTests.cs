
using Microsoft.Azure.WebJobs.Host.Bindings;
using Moq;
using Redis.WebJobs.Extensions.Bindings;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Tests
{
    public class RedisValueBinderTests
    {
        private const string ChannelName = "Channel";


        public async Task XXX()
        {
            // Arrange
            IValueBinder binder = CreateBinder<string>();

            // Act

            // Assert

        }

        private static RedisValueBinder<T> CreateBinder<T>()//out Mock<IRedisService> mockService, string partitionKey = null)
                where T : class
        {
            //mockService = new Mock<ICosmosDBService>(MockBehavior.Strict);

            RedisAttribute attribute = new RedisAttribute(ChannelName);

            var context = new RedisContext
            {
                ResolvedAttribute = attribute
                //Service = mockService.Object
            };

            return new RedisValueBinder<T>(context);
        }
    }
}
