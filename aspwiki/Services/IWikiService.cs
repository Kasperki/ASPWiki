using ASPWiki.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.Services
{
    public interface IWikiService
    {
        void Save(WikiPage wikiPage);
        List<Node> GetWikiTree(List<WikiPage> wikiPages);
        bool IsValidPath(string path, Guid id);
    }
}
