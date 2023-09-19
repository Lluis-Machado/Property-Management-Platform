using Microsoft.AspNetCore.Mvc;
using TaxManagementAPI.DTOs;

namespace TaxManagementAPI.Services
{
    public interface IDeclarantService
    {
        Task<DeclarantDTO> CreateDeclarantAsync(CreateDeclarantDTO declarantDTO, string userName);
        Task<ActionResult<DeclarantDTO>> UpdateDeclarantAsync(UpdateDeclarantDTO declarantDTO, Guid declarantId, string userName);
        Task<IEnumerable<DeclarantDTO>> GetPaginatedDeclarantsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<DeclarantDTO>> GetDeclarantsAsync(bool includeDeleted = false);
        Task<DeclarantDTO> DeleteDeclarantAsync(Guid declarantId, string userName);
        Task<DeclarantDTO> UndeleteDeclarantAsync(Guid declarantId, string userName);
        Task<DeclarantDTO?> DeclarantExists(Guid declarantId);

        // Removed to avoid compilation issues - Izar
        // MessageContract CreateContract(Guid id, string action, object oldObject, object newObject, string userName);
    }
}

