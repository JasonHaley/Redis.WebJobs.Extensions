
using Microsoft.Azure.WebJobs.Host.Executors;
using Redis.WebJobs.Extensions.Listeners;

namespace Redis.WebJobs.Extensions.Config
{
    public class RedisConfiguration
    {
        private bool _isConnectionStringInitialized = false;
        private string _connectionString;
        private readonly string _connectionStringName = "Redis";

        public RedisConfiguration(string connectionStringName)
            : this()
        {
            _connectionStringName = connectionStringName;
        }

        public RedisConfiguration()
        {
            ChannelMessageHandlerFactory = new DefaultChannelMessageHandlerFactory();
        }

        public string ConnectionString
        {
            get
            {
                if (!_isConnectionStringInitialized)
                {
                    _connectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(_connectionStringName);
                    _isConnectionStringInitialized = true;
                }

                return _connectionString;
            }
            set
            {
                _connectionString = value;
                _isConnectionStringInitialized = true;
            }
        }
        
        public IChannelMessageHandlerFactory ChannelMessageHandlerFactory { get; set; }
    }
}
