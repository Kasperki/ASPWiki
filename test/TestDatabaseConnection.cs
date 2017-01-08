using System;
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
            client = new MongoClient(GetClientSettings());
            database = client.GetDatabase(Constants.TestDatabaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }

        public MongoClientSettings GetClientSettings()
        {
            return new MongoClientSettings()
            {
                Server = new MongoServerAddress("localhost", Constants.DatabasePort),
            };
        }
    }
}
