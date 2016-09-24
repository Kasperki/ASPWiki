using ASPWiki;
using ASPWiki.Services;
using MongoDB.Driver;

namespace test
{
    public class TestDatabaseConnection : IDatabaseConnection
    {
        private MongoClient client;
        private IMongoDatabase database;

        public TestDatabaseConnection()
        {
            StartConnection();
        }

        public void StartConnection()
        {
            client = new MongoClient("mongodb://localhost:" + Constants.DatabasePort);
            database = client.GetDatabase(Constants.TestDatabaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }
    }
}
