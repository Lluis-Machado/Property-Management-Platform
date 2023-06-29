using MongoDB.Bson;
using MongoDB.Driver;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.Repositories
{
    public interface IPropertiesRepository
    {
        Task<Property> InsertOneAsync(Property property);
        Task<List<Property>> GetAsync();
        Task<Property> GetPropertyByIdAsync(Guid propertyId);
        Task<Property> GetByIdAsync(Guid propertyId);
        Task<Property> UpdateAsync(Property property);
        Task<UpdateResult> SetDeleteDeclarantAsync(Guid propertyId, bool deleted, string lastUser);
    }
}
