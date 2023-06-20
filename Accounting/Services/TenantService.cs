using AutoMapper;
using AccountingAPI.Repositories;
using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TenantService> _logger;

        public TenantService(ITenantRepository tenantRepository, ILogger<TenantService> logger, IMapper mapper)
        {
            _tenantRepository = tenantRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<TenantDTO> CreateTenantAsync(CreateTenantDTO createTenantDTO, string userName)
        {
            Tenant tenant = _mapper.Map<Tenant>(createTenantDTO);
            tenant.CreatedBy = userName;
            tenant.LastModificationBy = userName;

            tenant = await _tenantRepository.InsertTenantAsync(tenant);
            return _mapper.Map<TenantDTO>(tenant);  
        }
        public async Task<IEnumerable<TenantDTO>> GetTenantsAsync(bool includeDeleted = false)
        {
            IEnumerable<Tenant> tenants = await _tenantRepository.GetTenantsAsync(includeDeleted);
            return _mapper.Map<IEnumerable<Tenant>, List<TenantDTO>>(tenants);
        }

        public async Task<TenantDTO> GetTenantByIdAsync(Guid TenantId)
        {
            Tenant tenant = await _tenantRepository.GetTenantByIdAsync(TenantId);
            return _mapper.Map<TenantDTO>(tenant);
        }

        public async Task<bool> CheckIfTenantExistsAsync(Guid TenantId)
        {
            return await _tenantRepository.GetTenantByIdAsync(TenantId) != null;
        }

        public async Task<TenantDTO?> UpdateTenantAsync(CreateTenantDTO createTenantDTO, string userName, Guid tenantId)
        {
            Tenant tenant = _mapper.Map<Tenant>(createTenantDTO);
            tenant.Id = tenantId;
            tenant.LastModificationAt = DateTime.Now;
            tenant.LastModificationBy = userName;

            tenant = await _tenantRepository.UpdateTenantAsync(tenant);
           return _mapper.Map<TenantDTO>(tenant);
        }

        public async Task<int> SetDeletedTenantAsync(Guid tenantId, bool deleted)
        {
            return await _tenantRepository.SetDeletedTenantAsync(tenantId,deleted);
        }
    }
}