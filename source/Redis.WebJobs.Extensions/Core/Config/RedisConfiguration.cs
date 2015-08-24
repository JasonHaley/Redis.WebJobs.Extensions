using System;
using Microsoft.Azure.WebJobs.Host.Executors;

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
            LastValueKeyNamePrefix = "Previous_";
            CheckCacheFrequency = TimeSpan.FromSeconds(30);
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
        

        public TimeSpan? CheckCacheFrequency { get; set; }
        public string LastValueKeyNamePrefix { get; set; }
    }
}
