using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.WebJobs.Extensions
{
    public class RedisPubSubMessage
    {
        public string Channel { get; set; }
        public string Message { get; set; }
    }
}
