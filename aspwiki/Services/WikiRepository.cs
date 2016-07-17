using System.Linq;
using System.Collections.Generic;
using ASPWiki.Model;
using System;

namespace ASPWiki.Services
{
    public class WikiRepository : IWikiRepository
    {
        private List<WikiPage> wikiRepository;

        public WikiRepository()
        {
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
            var wiki = wikiRepository.FirstOrDefault(w => w.Id == wikiPage.Id);

            if (wiki != null)
            {
                Delete(wiki.Path.ToArray());
            }

            wikiRepository.Add(wikiPage); 
        }

        public void Delete(string[] path)
        {
            wikiRepository.Remove(GetByPath(path));
        }
    }
}
