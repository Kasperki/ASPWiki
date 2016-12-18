using ASPWiki.Infastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ASPWiki.Services
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private MongoClient client;
        private IMongoDatabase database;

        private readonly IOptions<ConfigurationOptions> options;


        public DatabaseConnection(IOptions<ConfigurationOptions> options)
        {
            this.options = options;
            StartConnection();
        }

        public void StartConnection()
        {
            var databaseCredentials = MongoCredential.CreateCredential(Constants.DatabaseName, options.Value.DatabaseUser, options.Value.DatabasePassword);

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
