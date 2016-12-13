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
            var databaseCredentials = MongoCredential.CreateCredential(Constants.DatabaseName, "root", "root");

            MongoClientSettings clienSettings = new MongoClientSettings()
            {
                Server = new MongoServerAddress("localhost", Constants.DatabasePort),
                Credentials = new[]
                {
                    databaseCredentials,
                }
            };

            client = new MongoClient(clienSettings);
            database = client.GetDatabase(Constants.DatabaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }
    }
}
