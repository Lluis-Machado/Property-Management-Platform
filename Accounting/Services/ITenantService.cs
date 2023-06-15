﻿using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface ITenantService
    {
        Task<TenantDTO> CreateTenantAsync(CreateTenantDTO createTenantDTO, string userName);
        Task<IEnumerable<TenantDTO>> GetTenantsAsync(bool includeDeleted = false);
        Task<TenantDTO> GetTenantByIdAsync(Guid TenantId);
        Task<bool> CheckIfTenantExistsAsync(Guid TenantId);
        Task<TenantDTO?> UpdateTenantAsync(CreateTenantDTO createTenantDTO, string userName, Guid tenantId);
        Task<int> SetDeletedTenantAsync(Guid tenantId, bool deleted);
    }
}
