using TaxManagement.Models;

namespace TaxManagement.Repositories
{
    public interface IDeclarantRepository
    {
        Task<Declarant> InsertDeclarantAsync(Declarant declarant);
        Task<IEnumerable<Declarant>> GetDeclarantsAsync();
        Task<Declarant?> GetDeclarantByIdAsync(Guid id);

        Task<int> UpdateDeclarantAsync(Declarant declarant);
        Task<int> SetDeleteDeclarantAsync(Guid id, bool deleted);
    }
}
