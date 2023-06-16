using MongoDB.Driver;
using OwnershipAPI.Contexts;
using OwnershipAPI.Models;
using OwnershipAPI.Repositories;

namespace OwnershipAPI.Services
{
    public class OwnershipRepository : IOwnershipRepository
    {
        private readonly IMongoCollection<Ownership> _collection;
        public OwnershipRepository(MongoContext context)
        {
            var database = context.GetDataBase("ownerships");
            _collection = database.GetCollection<Ownership>("ownerships");
        }

        public async Task<Ownership> InsertOneAsync(Ownership contact)
        {
            await _collection.InsertOneAsync(contact);
            return contact;
        }

        public async Task<List<Ownership>> GetAsync()
        {
            var filter = Builders<Ownership>.Filter.Empty;

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<List<Ownership>> GetWithContactIdAsync(Guid id)
        {
            var filter = Builders<Ownership>.Filter.Eq(c => c.ContactId, id);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<Ownership> GetOwnershipByIdAsync(Guid id)
        {
            var filter = Builders<Ownership>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<Ownership> UpdateAsync(Ownership contact)
        {
            var filter = Builders<Ownership>.Filter.Eq(actualContact => actualContact.Id, contact.Id);

            await _collection.ReplaceOneAsync(filter, contact);

            return contact;
        }

        public async Task<UpdateResult> SetDeleteAsync(Guid contactId, bool deleted)
        {
            var filter = Builders<Ownership>.Filter
                .Eq(actualContact => actualContact.Id, contactId);

            var update = Builders<Ownership>.Update
                .Set(actualContact => actualContact.Deleted, deleted);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
