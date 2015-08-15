using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions.Bindings
{
    internal class RedisPubSubEntity
    {
        public RedisAccount Account { get; set; }
        public string ChannelName { get; set; }
        public Task SendAsync(string message)
        {
            return Account.RedisDb.PublishAsync(ChannelName, message);
        }
    }
}
