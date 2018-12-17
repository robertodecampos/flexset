using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Exceptions
{
    public class SiteValidateException : Exception
    {
        public SiteValidateException(string message) : base(message) { }
    }
}
