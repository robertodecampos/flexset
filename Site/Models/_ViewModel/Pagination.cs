using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models._ViewModel
{
    public class Pagination
    {
        public int Page { get; set; } = 1;
        public int PeerPage { get; set; } = 10;
    }
}
