using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.ViewModels
{
    public class WikiPageMetadata
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }
    }
}
