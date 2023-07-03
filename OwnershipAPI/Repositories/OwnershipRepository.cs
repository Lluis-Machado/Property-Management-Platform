using MongoDB.Driver;
using OwnershipAPI.Contexts;
using OwnershipAPI.Models;
using OwnershipAPI.Repositories;

namespace OwnershipAPI.Services
{
    public class OwnershipRepository : IOwnershipRepository
    {
        private readonly IMongoCollection<Ownership> _collection;
        public OwnershipRepository(IMongoContext context)
        {
            var database = context.GetDatabase("ownerships");
            _collection = database.GetCollection<Ownership>("ownerships");
        }

        public async Task<Ownership> InsertOneAsync(Ownership ownership)
        {
            ownership.Id = Guid.NewGuid();
            await _collection.InsertOneAsync(ownership);
            return ownership;
        }

        public async Task<List<Ownership>> GetAsync()
        {
            var filter = Builders<Ownership>.Filter.Empty;
            filter = filter & Builders<Ownership>.Filter.Eq(c => c.Deleted, false);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<List<Ownership>> GetWithContactIdAsync(Guid id)
        {
            var filter = Builders<Ownership>.Filter.Eq(c => c.ContactId, id);
            filter = filter & Builders<Ownership>.Filter.Eq(c => c.Deleted, false);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<List<Ownership>> GetWithPropertyIdAsync(Guid id)
        {
            var filter = Builders<Ownership>.Filter.Eq(c => c.PropertyId, id);
            filter = filter & Builders<Ownership>.Filter.Eq(c => c.Deleted, false);

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

        public async Task<UpdateResult> SetDeleteAsync(Guid ownershipId, bool deleted)
        {
            var filter = Builders<Ownership>.Filter
                .Eq(ownershipContact => ownershipContact.Id, ownershipId);

            var update = Builders<Ownership>.Update
                .Set(ownershipContact => ownershipContact.Deleted, deleted);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
