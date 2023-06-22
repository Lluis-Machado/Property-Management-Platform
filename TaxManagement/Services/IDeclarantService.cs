using Microsoft.AspNetCore.Mvc;
using TaxManagement.Models;
using TaxManagement.Repositories;
using TaxManagement.Validators;
using TaxManagementAPI.DTOs;
using FluentValidation;
using FluentValidation.Results;
using MessagingContracts;

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
        MessageContract CreateContract(Guid id, string action, object oldObject, object newObject, string userName);
    }
}

