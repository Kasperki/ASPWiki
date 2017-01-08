using MongoDB.Driver;

namespace ASPWiki.Services
{
    public interface IDatabaseConnection
    {
        void StartConnection();

        IMongoDatabase GetDatabase();

        MongoClientSettings GetClientSettings();
    }
}
