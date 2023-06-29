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

        public async Task<Property> GetPropertyByIdAsync(Guid propertyId)
        {
            var filter = Builders<Property>.Filter.Eq(p => p.Id, propertyId);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
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

        public async Task<UpdateResult> SetDeleteDeclarantAsync(Guid propertyId, bool deleted, string lastUser)
        {
            var filter = Builders<Property>.Filter
                .Eq(actualProperty => actualProperty.Id, propertyId);

            var update = Builders<Property>.Update
                .Set(actualProperty => actualProperty.LastUpdateByUser, lastUser)
                .Set(actualProperty => actualProperty.LastUpdateAt, DateTime.UtcNow)
                .Set(actualProperty => actualProperty.Deleted, deleted);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
