
namespace Redis.WebJobs.Extensions
{
    internal interface IRedisAttribute
    {
        string ChannelOrKey { get; }
        Mode Mode { get; set; }
        string ConnectionStringSetting { get; set; }
    }
}
