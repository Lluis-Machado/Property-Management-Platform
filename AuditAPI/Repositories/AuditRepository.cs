using AuditsAPI.Contexts;
using AuditsAPI.Models;
using MongoDB.Driver;

namespace AuditsAPI.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly IMongoCollection<Audit> _collection;
        public AuditRepository(MongoContext context)
        {
            var database = context.GetDataBase("audits");
            _collection = database.GetCollection<Audit>("audits");
        }

        public async Task<Audit> InsertOneAsync(Audit autdit)
        {
            await _collection.InsertOneAsync(autdit);
            return autdit;
        }

        public async Task<List<Audit>> GetAsync(bool includeDeleted = false)
        {
            FilterDefinition<Audit> filter;

            if (includeDeleted)
            {
                filter = Builders<Audit>.Filter.Empty;
            }
            else
            {
                filter = Builders<Audit>.Filter.Eq(x => x.Deleted, false);
            }

            var result = await _collection.Find(filter).ToListAsync();
            return result;
        }


        public async Task<Audit> GetByIdAsync(Guid id)
        {
            var filter = Builders<Audit>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfAnyExistAsync(Guid id)
        {
            var filter = Builders<Audit>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .AnyAsync();
        }

        public async Task<Audit> UpdateAsync(Audit autdit)
        {
            var filter = Builders<Audit>.Filter.Eq(actualAutdit => actualAutdit.Id, autdit.Id);

            await _collection.ReplaceOneAsync(filter, autdit);

            return autdit;
        }

        public async Task<UpdateResult> SetDeleteAsync(Guid autditId, bool deleted, string lastUser)
        {
            var filter = Builders<Audit>.Filter
                .Eq(actualautdit => actualautdit.Id, autditId);

            var update = Builders<Audit>.Update
                .Set(actualAutdit => actualAutdit.Deleted, deleted)
                .Set(actualAutdit => actualAutdit.LastUpdateByUser, lastUser)
                .Set(actualAutdit => actualAutdit.LastUpdateAt, DateTime.UtcNow);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
