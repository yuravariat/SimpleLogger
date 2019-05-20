using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogger
{
    static class Ext
    {
        public static string PathCombine(this string val, string append)
        {
            if (string.IsNullOrEmpty(val)) return append;
            if (string.IsNullOrEmpty(append)) return val;
            return val.TrimEnd('\\') + "\\" + append.TrimStart('\\');
        }
    }
}
