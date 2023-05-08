using MongoDB.Driver;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.Services
{
    public class PropertiesRepository
    {
        private readonly IMongoCollection<Property> _logCollection;

        public PropertiesRepository(
            IOptions<LogDatabaseSettings> logsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                logsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                logsDatabaseSettings.Value.DatabaseName);

            _logCollection = mongoDatabase.GetCollection<Log>(
                logsDatabaseSettings.Value.LogsCollectionName);
        }

        public async Task<List<Log>> GetAsync() =>
            await _logCollection.Find(_ => true).ToListAsync();
    }
}
