using ASPWiki.Model;
using System;
using System.Collections.Generic;

namespace ASPWiki.Services
{
    public interface IWikiRepository
    {
        void Save(WikiPage wikiPage);
        void Delete(string path);
        bool Recover(string path);
        WikiPage GetById(Guid id);
        WikiPage GetByPath(string path);
        List<WikiPage> GetLatest(int number);
        List<WikiPage> GetAll();
    }
}
