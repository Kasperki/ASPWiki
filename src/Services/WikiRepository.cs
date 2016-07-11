using System.Linq;
using System.Collections.Generic;
using ASPWiki.Model;

namespace ASPWiki.Services
{
    public class WikiRepository : IWikiRepository
    {
        private Dictionary<string, WikiPage> wikiRepository;

        public WikiRepository()
        {
            wikiRepository = new Dictionary<string, WikiPage>();
        }

        public void Delete(string title)
        {
            wikiRepository.Remove(title);
        }

        public bool Exists(string title)
        {
            return wikiRepository.ContainsKey(title);
        }

        public WikiPage Get(string title)
        {
            if (wikiRepository.ContainsKey(title))
                return wikiRepository[title];

            return null;
        }

        public void Save(string title, WikiPage wikiPage)
        {
            wikiRepository[title] = wikiPage;
        }

        public List<WikiPage> GetLatest(int number)
        {
            var sortedList = (from entry in wikiRepository orderby entry.Value.LastModified descending select entry.Value).Take(number).ToList();
            return sortedList;
        }
    }
}
