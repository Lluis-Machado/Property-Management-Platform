using MongoDB.Driver;
using PropertiesAPI.Contexts;
using PropertiesAPI.Models;

namespace PropertiesAPI.Repositories
{
    public class PropertiesRepository : IPropertiesRepository
    {
        private readonly IMongoCollection<Property> _collection;
        public PropertiesRepository(MongoContext context)
        {
            var database = context.GetDataBase("properties");
            _collection = database.GetCollection<Property>("properties");
        }

        public async Task<Property> InsertOneAsync(Property property)
        {
            await _collection.InsertOneAsync(property);
            return property;
        }

        public async Task<List<Property>> GetAsync()
        {
            var filter = Builders<Property>.Filter.Empty;
            filter = filter & Builders<Property>.Filter.Eq(c => c.Deleted, false);

            var results = await _collection.Find(filter).ToListAsync();
            return results;
        }

        public async Task<Property> GetPropertyByIdAsync(Guid propertyId)
        {
            var filter = Builders<Property>.Filter.Eq(p => p.Id, propertyId);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Property>> GetPropertiesByParentIdAsync(Guid parentPropertyId)
        {
            var filter = Builders<Property>.Filter.Eq(p => p.MainPropertyId, parentPropertyId);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<List<Property>> GetSidePropertiesByParentIdAsync(Guid parentPropertyId)
        {
            var filter = Builders<Property>.Filter.Eq(p => p.MainPropertyId, parentPropertyId);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<Property> GetByIdAsync(Guid propertyId)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty.Id, propertyId);

            return await _collection.Find(filter)
                .FirstAsync();
        }

        public async Task<Property> UpdateAsync(Property property)
        {
            var filter = Builders<Property>.Filter.Eq(actualProperty => actualProperty.Id, property.Id);

            await _collection.ReplaceOneAsync(filter, property);

            return property;
        }

        public async Task<UpdateResult> UpdateParentIdAsync(Guid parentId, Guid childId)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty.Id, childId);

            var update = Builders<Property>.Update
                .Set(actualProperty => actualProperty.MainPropertyId, parentId);

            return await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UpdateArchiveIdAsync(Guid propertyId, Guid archiveId, string username)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty.Id, propertyId);

            var update = Builders<Property>.Update
                .Set(actualProperty => actualProperty.ArchiveId, archiveId)
                .Set(actualProperty => actualProperty.LastUpdateAt, DateTime.UtcNow)
                .Set(actualProperty => actualProperty.LastUpdateByUser, username);

            return await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UpdateNameAsync(Guid propertyId, string name, string username)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty.Id, propertyId);

            var update = Builders<Property>.Update
                .Set(actualProperty => actualProperty.Name, name)
                .Set(actualProperty => actualProperty.LastUpdateAt, DateTime.UtcNow)
                .Set(actualProperty => actualProperty.LastUpdateByUser, username);

            return await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> SetDeleteAsync(Guid propertyId, bool deleted, string lastUser)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty.Id, propertyId);

            var update = Builders<Property>.Update
                .Set(actualProperty => actualProperty.LastUpdateByUser, lastUser)
                .Set(actualProperty => actualProperty.LastUpdateAt, DateTime.UtcNow)
                .Set(actualProperty => actualProperty.Deleted, deleted);

            return await _collection.UpdateOneAsync(filter, update);
        }
        public async Task<UpdateResult> SetMainOwner(Guid propertyId, Guid ownerId, string ownerType, string lastUser)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty.Id, propertyId);

            var update = Builders<Property>.Update
                .Set(actualProperty => actualProperty.LastUpdateByUser, lastUser)
                .Set(actualProperty => actualProperty.LastUpdateAt, DateTime.UtcNow)
                //  .Set(actualProperty => actualProperty.MainOwnerType, ownerType)
                .Set(actualProperty => actualProperty.ContactPersonId, ownerId);

            return await _collection.UpdateOneAsync(filter, update);
        }


    }
}
