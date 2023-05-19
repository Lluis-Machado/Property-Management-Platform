using MongoDB.Bson;
using MongoDB.Driver;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.Repositories
{
    public interface IPropertiesRepository
    {
        Task<Property> InsertOneAsync(Property property);
        Task<List<Property>> GetAsync();
        Task<List<Property>> GetByContactIdAsync(Guid contactId);
        Task<Property> GetByIdAsync(Guid propertyId);
        Task<UpdateResult> UpdateAsync(Property property);
        Task<UpdateResult> SetDeleteDeclarantAsync(Guid property, bool deleted);
    }
}
