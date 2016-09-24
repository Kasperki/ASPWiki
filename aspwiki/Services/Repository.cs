using MongoDB.Driver;

namespace ASPWiki.Services
{
    public class Repository<T>
    {
        protected IMongoDatabase database;
        protected IMongoCollection<T> collection;

        public Repository(IDatabaseConnection databaseConnection, string collectionName)
        {
            database = databaseConnection.GetDatabase();
            collection = database.GetCollection<T>(collectionName);
        }
    }
}
