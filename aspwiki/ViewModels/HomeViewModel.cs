using ASPWiki.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.ViewModels
{
    public class HomeViewModel
    {
        public List<WikipageSummary> LatestWikipages { get; set; }

        public List<WikipageSummary> PopularWikipages { get; set; }
    }
}
