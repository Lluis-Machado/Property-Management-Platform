using MongoDB.Bson;
using MongoDB.Driver;
using PropertiesAPI.Models;

namespace PropertiesAPI.Repositories
{
    public interface IPropertiesRepository
    {
        Task<Property> InsertOneAsync(Property property);
        Task<List<Property>> GetAsync();
        Task<Property> GetPropertyByIdAsync(Guid propertyId);
        Task<List<Property>> GetPropertiesByParentIdAsync(Guid parentPropertyId);
        Task<Property> GetByIdAsync(Guid propertyId);
        Task<Property> UpdateAsync(Property property);
        Task<UpdateResult> SetDeleteDeclarantAsync(Guid propertyId, bool deleted, string lastUser);
        Task<UpdateResult> UpdateParentIdAsync(Guid parentId, Guid childId);
    }
}
