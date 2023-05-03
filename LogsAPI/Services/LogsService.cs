using LogsAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace LogsAPI.Services
{
    public class LogsService
    {
        private readonly IMongoCollection<Log> _logCollection;

        public LogsService(
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
