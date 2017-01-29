using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;

namespace Redis.WebJobs.Extensions.Framework
{
    public class ConsoleTraceWriter : TraceWriter
    {
        public ConsoleTraceWriter(TraceLevel level) : base(level)
        {
        }

        public override void Trace(TraceLevel level, string source, string message, Exception ex)
        {
            if (ex == null)
            {
                Console.WriteLine("Level: {0}, Source: {1}, Message: {2}", level, source, message);
            }
            else
            {
                Console.WriteLine("Level: {0}, Source: {1}, Message: {2}, Exception: {3}", level, source, message, ex.Message);
            }
            
        }
    }
}
