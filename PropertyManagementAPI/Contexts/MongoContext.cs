using MongoDB.Bson;
using MongoDB.Driver;

namespace PropertiesAPI.Contexts
{
    public class MongoContext : IMongoContext
    {
        private readonly string? _connectionString;

        public MongoContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MongoConnection");
        }

        public IMongoDatabase GetDataBase(string databaseName)
        {
            MongoClient dbClient = new(_connectionString);
            return dbClient.GetDatabase(databaseName);
        }
    }
}