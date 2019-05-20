using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogger
{
    public enum LogLevel : short
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 4,
        Error = 8,
        Critical = 16
    }
}
