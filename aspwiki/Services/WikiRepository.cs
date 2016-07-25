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

        public WikiPage GetByPath(string path)
        {
            if (path != null)
            {
                foreach (WikiPage wikiPage in wikiRepository)
                {
                    if (String.Equals(wikiPage.Path, path))
                        return wikiPage;
                }
            }

            return null;
        }

        public List<WikiPage> GetLatest(int number, bool authenticated)
        {
            var query = from wikiPage in wikiRepository select wikiPage;

            if (!authenticated)
                query = query.Where(wikiPage => wikiPage.Public == true);

            query.OrderByDescending(wikiPage => wikiPage.LastModified);

            return query.Take(number).ToList();
        }

        public List<WikiPage> GetAll(bool authenticated)
        {
            if (authenticated)
            {
                return wikiRepository.ToList();
            }
            else
            {
                return (from wikiPage in wikiRepository where wikiPage.Public == true select wikiPage).ToList();
            }
        }

        public void Save(WikiPage wikiPage)
        {
            var wiki = GetById(wikiPage.Id);

            if (wiki != null)
            {
                Delete(wiki.Path);
            }

            wikiRepository.Add(wikiPage); 
        }

        public void Delete(string path)
        {
            temporalWikiDir.Add(DateTime.Now, GetByPath(path));

            wikiRepository.Remove(GetByPath(path));
        }

        public bool Recover(string path)
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

                if (String.Equals(wikiPage.Value.Path, path))
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
