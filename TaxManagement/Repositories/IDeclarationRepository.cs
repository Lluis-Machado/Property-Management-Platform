using TaxManagement.Models;

namespace TaxManagement.Repositories
{
    public interface IDeclarationRepository
    {
        public Task<IEnumerable<Declaration>> GetDeclarationsAsync(Guid? declarantId = null);
        Task<Guid> InsertDeclarationAsync(Declaration declaration);
        Task<Declaration> GetDeclarationByIdAsync(Guid id, Guid? declarantId = null);
        Task<int> UpdateDeclarationAsync(Declaration declaration);

    }
}
