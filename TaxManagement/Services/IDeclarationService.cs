using TaxManagementAPI.DTOs;

namespace TaxManagementAPI.Services
{
    public interface IDeclarationService
    {
        Task<DeclarationDTO> CreateDeclarationAsync(CreateDeclarationDTO declaration, string userName);
        Task<DeclarationDTO> UpdateDeclarationAsync(UpdateDeclarationDTO declarationDTO);
        Task<DeclarationDTO> SetDeletedDeclarationAsync(Guid declarantId, Guid declarationId, bool delete, string userName);
        Task<IEnumerable<DeclarationDTO>> GetDeclarationsAsync(Guid declarantId, bool includeDeleted = false);
        Task<bool> DeclarationExists(Guid declarantId);
        Task<DeclarationDTO> DeleteDeclarationAsync(Guid declarantId, Guid declarationId, string userName);
    }
}

