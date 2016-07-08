using ASPWiki.Model;

namespace ASPWiki.Services
{
    public interface IWikiRepository
    {
        void Save(string title, WikiPage wikiPage);
        void Delete(string title);
        bool Exists(string title);
        WikiPage Get(string title);
    }
}
