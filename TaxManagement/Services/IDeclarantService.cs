using Microsoft.AspNetCore.Mvc;
using TaxManagement.Models;
using TaxManagement.Repositories;
using TaxManagement.Validators;
using TaxManagementAPI.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace TaxManagementAPI.Services
{
    public interface IDeclarantService
    {
        Task<DeclarantDTO> CreateDeclarantAsync(CreateDeclarantDTO declarantDTO, string userName);
        Task<ActionResult<DeclarantDTO>> UpdateDeclarantAsync(UpdateDeclarantDTO declarantDTO, Guid declarantId, string lastUpdateByUser);
        Task<IEnumerable<DeclarantDTO>> GetPaginatedDeclarantsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<DeclarantDTO>> GetDeclarantsAsync();
        Task<DeclarantDTO> DeleteDeclarantAsync(Guid declarantId, string lastUserName);
        Task<DeclarantDTO> UndeleteDeclarantAsync(Guid declarantId, string lastUserName);
        Task<bool> DeclarantExists(Guid declarantId);
    }
}

