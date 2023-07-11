using LogsAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace LogsAPI.Services
{
    public class LogsRepository
    {
        private readonly IMongoCollection<Log> _logCollection;

        public LogsRepository(
            IOptions<LogDatabaseSettings> logsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                logsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                logsDatabaseSettings.Value.DatabaseName);

            _logCollection = mongoDatabase.GetCollection<Log>(
                logsDatabaseSettings.Value.LogsCollectionName);
        }

        public async Task<List<Log>> GetAsync(DateTime? periodStart, DateTime? periodEnd)
        {
            var filter = Builders<Log>.Filter.Empty;

            if (periodStart.HasValue)
            {
                filter &= Builders<Log>.Filter.Gte(log => log.Timestamp, periodStart.Value);
            }

            if (periodEnd.HasValue)
            {
                filter &= Builders<Log>.Filter.Lte(log => log.Timestamp, periodEnd.Value);
            }

            return await _logCollection.Find(filter).ToListAsync();
        }
    }
}
