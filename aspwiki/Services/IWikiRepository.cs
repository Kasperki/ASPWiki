using ASPWiki.Model;
using System.Collections.Generic;

namespace ASPWiki.Services
{
    public interface IWikiRepository
    {
        void Save(WikiPage wikiPage);
        void Delete(string[] path);
        WikiPage GetByPath(string[] path);
        List<WikiPage> GetLatest(int number);
        List<WikiPage> GetAll();
    }
}
