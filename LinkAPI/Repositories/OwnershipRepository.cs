using LinkAPI.Contexts;
using LinkAPI.Models;
using MongoDB.Driver;

namespace LinkAPI.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly IMongoCollection<Link> _collection;
        public LinkRepository(IMongoContext context)
        {
            var database = context.GetDatabase("links");
            _collection = database.GetCollection<Link>("links");
        }

        public async Task<Link> InsertOneAsync(Link ownership)
        {
            ownership.Id = Guid.NewGuid();
            await _collection.InsertOneAsync(ownership);
            return ownership;
        }

        public async Task<List<Link>> GetAsync()
        {
            var filter = Builders<Link>.Filter.Empty;
            filter = filter & Builders<Link>.Filter.Eq(c => c.Deleted, false);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<List<Link>> GetWithObjectAIdAsync(Guid id)
        {
            var filter = Builders<Link>.Filter.Eq(c => c.ObjectAId, id);
            filter = filter & Builders<Link>.Filter.Eq(c => c.Deleted, false);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<List<Link>> GetWithObjectBIdAsync(Guid id)
        {
            var filter = Builders<Link>.Filter.Eq(c => c.ObjectBId, id);
            filter = filter & Builders<Link>.Filter.Eq(c => c.Deleted, false);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<Link> GetLinkByIdAsync(Guid id)
        {
            var filter = Builders<Link>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<Link> UpdateAsync(Link contact)
        {
            var filter = Builders<Link>.Filter.Eq(actualContact => actualContact.Id, contact.Id);

            await _collection.ReplaceOneAsync(filter, contact);

            return contact;
        }

        public async Task<UpdateResult> SetDeleteAsync(Guid ownershipId, bool deleted)
        {
            var filter = Builders<Link>.Filter
                .Eq(ownershipContact => ownershipContact.Id, ownershipId);

            var update = Builders<Link>.Update
                .Set(ownershipContact => ownershipContact.Deleted, deleted);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
