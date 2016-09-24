using MongoDB.Driver;

namespace ASPWiki.Services
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private MongoClient client;
        private IMongoDatabase database;

        public DatabaseConnection()
        {
            StartConnection();
        }

        public void StartConnection()
        {
            client = new MongoClient("mongodb://localhost:" + Constants.DatabasePort);
            database = client.GetDatabase(Constants.DatabaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }
    }
}
