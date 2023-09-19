using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AccountingAPI.Utilities;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IValidator<CreateTenantDTO> _createTenantDTOValidator;
        private readonly IValidator<UpdateTenantDTO> _updateTenantDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<TenantService> _logger;

        public TenantService(ITenantRepository tenantRepository, IValidator<CreateTenantDTO> createTenantDTOValidator, ILogger<TenantService> logger, IMapper mapper, IValidator<UpdateTenantDTO> updateTenantDTOValidator)
        {
            _tenantRepository = tenantRepository;
            _createTenantDTOValidator = createTenantDTOValidator;
            _logger = logger;
            _mapper = mapper;
            _updateTenantDTOValidator = updateTenantDTOValidator;
        }

        public async Task<TenantDTO> CreateTenantAsync(CreateTenantDTO createTenantDTO, string userName)
        {
            // validation
            await _createTenantDTOValidator.ValidateAndThrowAsync(createTenantDTO);

            Tenant tenant = _mapper.Map<Tenant>(createTenantDTO);
            tenant.CreatedBy = userName;
            tenant.LastModificationBy = userName;

            tenant = await _tenantRepository.InsertTenantAsync(tenant);

            return _mapper.Map<TenantDTO>(tenant);
        }

        public async Task<IEnumerable<TenantDTO>> GetTenantsAsync(bool includeDeleted = false, int? page = null, int? pageSize = null)
        {
            IEnumerable<Tenant> tenants = await _tenantRepository.GetTenantsAsync(includeDeleted);

            Pagination.Paginate(ref tenants, page, pageSize);

            return _mapper.Map<IEnumerable<Tenant>, List<TenantDTO>>(tenants);
        }

        public async Task<TenantDTO> GetTenantByIdAsync(Guid TenantId)
        {
            Tenant? tenant = await _tenantRepository.GetTenantByIdAsync(TenantId);

            if (tenant is null) throw new NotFoundException("Tenant");

            return _mapper.Map<TenantDTO>(tenant);
        }

        public async Task<TenantDTO> UpdateTenantAsync(Guid tenantId, UpdateTenantDTO updateTenantDTO, string userName)
        {
            // validation
            await _updateTenantDTOValidator.ValidateAndThrowAsync(updateTenantDTO);

            // check if exists
            await GetTenantByIdAsync(tenantId);

            Tenant tenant = _mapper.Map<Tenant>(updateTenantDTO);
            tenant.Id = tenantId;
            tenant.Name = updateTenantDTO.Name;
            tenant.LastModificationAt = DateTime.Now;
            tenant.LastModificationBy = userName;

            tenant = await _tenantRepository.UpdateTenantAsync(tenant);

            return _mapper.Map<TenantDTO>(tenant);
        }

        public async Task SetDeletedTenantAsync(Guid tenantId, bool deleted, string userName)
        {
            // check if exists
            TenantDTO tenantDTO = await GetTenantByIdAsync(tenantId);

            // check if already deleted/undeleted
            if (tenantDTO.Deleted == deleted)
            {
                string action = deleted ? "deleted" : "undeleted";
                throw new ConflictException($"Tenant already {action}");
            }

            await _tenantRepository.SetDeletedTenantAsync(tenantId, deleted, userName);
        }
    }
}