using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PropertyManagementAPI.Contexts;
using PropertyManagementAPI.Models;
using PropertyManagementAPI.Repositories;

namespace PropertyManagementAPI.Services
{
    public class PropertiesRepository : IPropertiesRepository
    {
        private readonly IMongoCollection<Property> _collection;
        public PropertiesRepository(MongoContext context)
        {
            var database = context.GetDataBase("propertyManagement");
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

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<List<Property>> GetByContactIdAsync(Guid contactId)
        {
            IMongoQueryable<Property> properties = _collection.AsQueryable();
            return await properties.Where(p => p.Owners.Any(o => o.ContactId == contactId))
                   .ToListAsync();
        }

        public async Task<Property> GetByIdAsync(Guid propertyId)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty._id, propertyId);

            return await _collection.Find(filter)
                .FirstAsync();
        }

        public async Task<UpdateResult> UpdateAsync(Property property)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty._id, property._id);

            var update = Builders<Property>.Update
                .Set(actualProperty => actualProperty.Name, property.Name)
                .Set(actualProperty => actualProperty.Address, property.Address);

            return await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> SetDeleteDeclarantAsync(Guid propertyId, bool deleted)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty._id, propertyId);

            var update = Builders<Property>.Update
                .Set(actualProperty => actualProperty.Deleted, deleted);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
