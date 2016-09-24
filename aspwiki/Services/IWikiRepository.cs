using ASPWiki.Model;
using System;
using System.Collections.Generic;

namespace ASPWiki.Services
{
    public interface IWikiRepository
    {
        void Add(WikiPage obj);
        void Update(WikiPage obj);
        void Delete(WikiPage obj);
        WikiPage GetById(Guid id);
        List<WikiPage> GetAll();

        void AddVisit(WikiPage wikipage);
        void RemoveFile(WikiPage wikipage, Guid fileId);
        bool Recover(string path);
        WikiPage GetByPath(string path);
        List<WikiPage> GetLatest(int number);
        List<WikiPage> GetPopular(int number);
        List<WikiPage> SearchByTitle(string keywords);
    }
}
