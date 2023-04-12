using TaxManagement.Models;

namespace TaxManagement.Repositories
{
    public interface IDeclarantRepository
    {
        Task<Guid> InsertDeclarantAsync(Declarant declarant);
        Task<IEnumerable<Declarant>> GetDeclarantsAsync();
        Task<Declarant> GetDeclarantByIdAsync(Guid id);

        Task<int> UpdateDeclarantAsync(Declarant declarant);
    }
}
