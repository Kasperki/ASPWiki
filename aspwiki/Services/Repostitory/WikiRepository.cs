using MongoDB.Driver;
using System.Collections.Generic;
using ASPWiki.Model;
using System;
using MongoDB.Bson;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ASPWiki.Services
{
    public class WikiRepository : Repository<WikiPage>, IWikiRepository
    {
        private readonly IHttpContextAccessor context;
        private readonly IAuthenticationService authenticationService;
        private Dictionary<SessionPath, WikiPage> deletedDictionary;

        public WikiRepository(IDatabaseConnection databaseConnection, IHttpContextAccessor context, IAuthenticationService authenticationService) :base(databaseConnection, Constants.WikiPagesCollectionName)
        {
            deletedDictionary = new Dictionary<SessionPath, WikiPage>();
            this.context = context;
            this.authenticationService = authenticationService;
        }

        public void Delete(WikiPage wikipage, bool revertable = true)
        {
            if (revertable)
            {
                AddWikipageForRecover(wikipage);
            }

            var builder = Builders<WikiPage>.Filter;
            var filter = builder.Eq(x => x.Id, wikipage.Id);

            collection.DeleteOne(filter);
        }

        private void AddWikipageForRecover(WikiPage wikipage)
        {
            deletedDictionary[new SessionPath(context.HttpContext.Session.Id, wikipage.Path, DateTime.Now)] = wikipage;

            //Delete old wikipages from recover list
            var itemsToBeRemoved = new List<SessionPath>();
            foreach (KeyValuePair<SessionPath, WikiPage> item in deletedDictionary)
            {
                if (item.Key.created + new TimeSpan(0, WikiPage.REVERTABLE_IN_MINUTES, 0) < DateTime.Now)
                {
                    itemsToBeRemoved.Add(item.Key);
                }
            }

            foreach (var item in itemsToBeRemoved)
            {
                deletedDictionary.Remove(item);
            }
        }

        public List<WikiPage> GetAll()
        {
            if (authenticationService.IsAuthenticatedAndWhiteListed())
            {
                return collection.Find(_ => true).ToList();
            }
            else
            {
                return collection.Find(x => x.Public ==  true).ToList();
            }
        }

        public List<WikiPage> GetAllPrivate()
        {
            return collection.Find(_ => true).ToList();
        }

        public WikiPage GetById(Guid id)
        {
            var builder = Builders<WikiPage>.Filter;
            var filter = builder.Eq(x => x.Id, id);

            return collection.Find(filter).FirstOrDefault();
        }

        public WikiPage GetByPath(string path)
        {
            var builder = Builders<WikiPage>.Filter;

            path = System.Net.WebUtility.UrlDecode(path);

            var filter = builder.Eq(x => x.Path, path);
            return collection.Find(filter).FirstOrDefault();
        }

        public List<WikiPage> GetLatest(int limit)
        {
            var sort = Builders<WikiPage>.Sort.Descending("LastModified");

            if (authenticationService.IsAuthenticatedAndWhiteListed())
            {
                return collection.Find(new BsonDocument()).Sort(sort).ToList().Take(limit).ToList();
            }
            else
            {
                return collection.Find(x => x.Public == true).Sort(sort).ToList().Take(limit).ToList();
            }
          
        }

        public List<WikiPage> GetPopular(int limit)
        {
            var sort = Builders<WikiPage>.Sort.Descending("Visits");
          
            if (authenticationService.IsAuthenticatedAndWhiteListed())
            {
                return collection.Find(new BsonDocument()).Sort(sort).ToList().Take(limit).ToList();
            }
            else
            {
                return collection.Find(x => x.Public == true).Sort(sort).ToList().Take(limit).ToList();
            }
        }

        public bool Recover(string path)
        {
            if (!deletedDictionary.ContainsKey(new SessionPath(context.HttpContext.Session.Id, path)))
                return false;

            Add(deletedDictionary[new SessionPath(context.HttpContext.Session.Id, path)]);
            return true;
        }

        public void Update(WikiPage wikiPage)
        {
            var builder = Builders<WikiPage>.Filter;
            var filter = builder.Eq(x => x.Id, wikiPage.Id);
            collection.ReplaceOne(filter, wikiPage);
        }

        public List<WikiPage> SearchByTitle(string keywords)
        {
            var all = GetAll();
            List<WikiPage> list = new List<WikiPage>();

            foreach (var item in all)
            {
                if ((item.Public || authenticationService.IsAuthenticatedAndWhiteListed()) && item.Title.Contains(keywords))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public void Add(WikiPage wikiPage)
        {
            collection.InsertOne(wikiPage);
        }

        public void AddVisit(WikiPage wikipage)
        {
            var builder = Builders<WikiPage>.Filter;
            var filter = builder.Eq(x => x.Id, wikipage.Id);
            var update = Builders<WikiPage>.Update;
            var updateDefinition = update.Set("Visits", wikipage.Visits++);
            collection.UpdateOne(filter, updateDefinition);
        }

        public void RemoveFile(WikiPage wikipage, Guid fileId)
        {
            var builder = Builders<WikiPage>.Filter;
            var filter = builder.Eq(x => x.Id, wikipage.Id);
            var update = Builders<WikiPage>.Update;
            wikipage.RemoveAttacment(fileId);
            var updateDefinition = update.Set("Attachments", wikipage.Attachments);
            collection.UpdateOne(filter, updateDefinition);
        }
    }
}