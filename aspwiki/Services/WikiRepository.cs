using System.Linq;
using System.Collections.Generic;
using ASPWiki.Model;
using System;

namespace ASPWiki.Services
{
    public class WikiRepository : IWikiRepository
    {
        private Dictionary<DateTime, WikiPage> temporalWikiDir;
        private List<WikiPage> wikiRepository;

        public WikiRepository()
        {
            temporalWikiDir = new Dictionary<DateTime, WikiPage>();
            wikiRepository = new List<WikiPage>();
        }

        public WikiPage GetByPath(string[] path)
        {
            if (path != null)
            {
                foreach (WikiPage wikiPage in wikiRepository)
                {
                    if (Enumerable.SequenceEqual(wikiPage.Path.ToArray(), path))
                        return wikiPage;
                }
            }

            return null;
        }

        public List<WikiPage> GetLatest(int number)
        {
            var sortedList = (from entry in wikiRepository orderby entry.LastModified descending select entry).Take(number).ToList();
            return sortedList;
        }

        public List<WikiPage> GetAll()
        {
            return wikiRepository.ToList();
        }

        public void Save(WikiPage wikiPage)
        {
            var wiki = GetById(wikiPage.Id);

            if (wiki != null)
            {
                Delete(wiki.Path.ToArray());
            }

            wikiRepository.Add(wikiPage); 
        }

        public void Delete(string[] path)
        {
            temporalWikiDir.Add(DateTime.Now, GetByPath(path));

            wikiRepository.Remove(GetByPath(path));
        }

        public bool Recover(string[] path)
        {
            bool recovered = false;
            List<DateTime> wikiPageToBeRemoved = new List<DateTime>();

            foreach (KeyValuePair<DateTime, WikiPage> wikiPage in temporalWikiDir)
            {
                if (wikiPage.Key.AddDays(1) < DateTime.Now)
                {
                    wikiPageToBeRemoved.Add(wikiPage.Key);
                    continue;
                }

                if (Enumerable.SequenceEqual(wikiPage.Value.Path.ToArray(), path))
                {
                    Save(wikiPage.Value);
                    wikiPageToBeRemoved.Add(wikiPage.Key);
                    recovered = true;
                }
            }

            foreach(var key in wikiPageToBeRemoved)
            {
                temporalWikiDir.Remove(key);
            }

            return recovered;
        }

        public WikiPage GetById(Guid id)
        {
            return wikiRepository.FirstOrDefault(w => w.Id == id);
        }
    }
}
