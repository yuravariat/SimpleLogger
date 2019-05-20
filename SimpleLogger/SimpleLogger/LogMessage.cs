using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogger
{
    class LogMessage
    {
        public LogLevel Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public object[] Parameters { get; set; }
    }
}
