using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Exceptions
{
    public class SiteException : Exception
    {
        public SiteException(string message) : base (message) { }
    }
}
