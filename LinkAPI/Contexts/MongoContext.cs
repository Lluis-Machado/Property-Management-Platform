using MongoDB.Driver;

namespace LinkAPI.Contexts
{
    public class MongoContext : IMongoContext
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public MongoContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString(name: "MongoConnection");
        }

        public IMongoDatabase GetDatabase(string databaseName)
        {
            MongoClient dbClient = new(_connectionString);
            return dbClient.GetDatabase(databaseName);
        }
    }
}
