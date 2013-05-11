using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployScript
{
    static class Extensions
    {
        public static bool HasValue(this string str)
        {
            return !String.IsNullOrWhiteSpace(str);
        }
    }
}
