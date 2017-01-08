using System;
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
            client = new MongoClient(GetClientSettings());
            database = client.GetDatabase(Constants.DatabaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }

        public MongoClientSettings GetClientSettings()
        {
            var databaseCredentials = MongoCredential.CreateCredential(Constants.DatabaseName, options.Value.DatabaseUser, options.Value.DatabasePassword);

            return new MongoClientSettings()
            {
                Server = new MongoServerAddress("localhost", Constants.DatabasePort),
                Credentials = new[]
                {
                    databaseCredentials,
                }
            };
        }
    }
}
