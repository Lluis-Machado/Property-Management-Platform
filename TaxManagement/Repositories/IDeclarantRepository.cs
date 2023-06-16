using TaxManagement.Models;
using TaxManagementAPI.DTOs;

namespace TaxManagement.Repositories
{
    public interface IDeclarantRepository
    {
        Task<Declarant> InsertDeclarantAsync(Declarant declarant);
        //Task<IEnumerable<Declarant>> GetDeclarantsAsync();
        Task<IEnumerable<Declarant>> GetDeclarantsAsync();
        Task<Declarant?> GetDeclarantByIdAsync(Guid id);

        Task<Declarant> UpdateDeclarantAsync(Declarant declarant);
        Task<Declarant> SetDeleteDeclarantAsync(Guid id, bool deleted, string? updatedUser);
    }
}
