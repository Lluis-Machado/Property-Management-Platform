using TaxManagement.Models;
using TaxManagementAPI.DTOs;

namespace TaxManagement.Repositories
{
    public interface IDeclarationRepository
    {
        public Task<IEnumerable<Declaration>> GetDeclarationsAsync(Guid declarantId, bool includeDeleted = false);
        Task<Declaration> InsertDeclarationAsync(Declaration declaration);
        Task<Declaration> GetDeclarationByIdAsync(Guid id, Guid? declarantId = null);
        Task<Declaration> UpdateDeclarationAsync(Declaration declaration);
        Task<Declaration> SetDeletedDeclarationAsync(Guid declarantId, Guid declarationId, bool deleted, string? userName);

    }
}
