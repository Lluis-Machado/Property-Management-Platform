using MongoDB.Driver;

namespace ContactsAPI.Contexts
{
    public class MongoContext : IMongoContext
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;
        public MongoContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MongoConnection");
        }
        public IMongoDatabase GetDataBase(string databaseName)
        {
            MongoClient dbClient = new(_connectionString);
            return dbClient.GetDatabase(databaseName);
        }
    }
}
