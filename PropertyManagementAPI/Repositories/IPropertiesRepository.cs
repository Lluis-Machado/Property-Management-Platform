using MongoDB.Bson;
using MongoDB.Driver;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.Repositories
{
    public interface IPropertiesRepository
    {
        Task<Property> CreateAsync(Property property);
        Task<List<Property>> GetAsync();
        Task<UpdateResult> UpdateAsync(Property property);
        Task<UpdateResult> SetDeleteDeclarantAsync(Guid property, bool deleted);
    }
}
