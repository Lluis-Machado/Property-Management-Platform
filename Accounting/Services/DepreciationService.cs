using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AccountingAPI.Utilities;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class DepreciationService : IDepreciationService
    {
        private readonly IDepreciationRepository _depreciationRepository;
        private readonly IValidator<CreateDepreciationDTO> _createDepreciationDTOValidator;
        private readonly IValidator<UpdateDepreciationDTO> _updateDepreciationDTOValidator;
        private readonly IPeriodService _periodService;
        private readonly IARInvoiceService _arInvoiceService;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IMapper _mapper;
        private readonly ILogger<DepreciationService> _logger;

        public DepreciationService(IDepreciationRepository depreciationRepository, IValidator<CreateDepreciationDTO> createDepreciationDTOValidator, IValidator<UpdateDepreciationDTO> updateDepreciationDTOValidator, ILogger<DepreciationService> logger, IMapper mapper, IFixedAssetService fixedAssetService, IPeriodService periodService, IARInvoiceService arInvoiceService)
        {
            _depreciationRepository = depreciationRepository;
            _createDepreciationDTOValidator = createDepreciationDTOValidator;
            _updateDepreciationDTOValidator = updateDepreciationDTOValidator;
            _periodService = periodService;
            _logger = logger;
            _mapper = mapper;
            _fixedAssetService = fixedAssetService;
            _arInvoiceService = arInvoiceService;
        }

        public async Task<DepreciationDTO> CreateDepreciationAsync(Guid tenantId, CreateDepreciationDTO createDepreciationDTO, Guid fixedAssetId, Guid periodId, string userName)
        {
            // validation
            await _createDepreciationDTOValidator.ValidateAndThrowAsync(createDepreciationDTO);

            // check that Fixed Asset Exists
            await _fixedAssetService.GetFixedAssetByIdAsync(tenantId, fixedAssetId);

            // check that Period Exists
            await _periodService.GetPeriodByIdAsync(tenantId, periodId);

            Depreciation depreciation = new()
            {
                FixedAssetId = fixedAssetId,
                PeriodId = periodId,
                DepreciationAmount = createDepreciationDTO.DepreciationAmount,
                CreatedBy = userName,
                LastModificationBy = userName
            };
            depreciation = await _depreciationRepository.InsertDepreciationAsync(depreciation);

            DepreciationDTO depreciationDTO = _mapper.Map<DepreciationDTO>(depreciation);

            return await MapDepreciationPeriodData(tenantId, depreciationDTO);
        }

        public async Task<IEnumerable<DepreciationDTO>> GetDepreciationsAsync(Guid tenantId)
        {
            IEnumerable<PeriodDTO> periodDTOs = await _periodService.GetPeriodsAsync(tenantId);
            IEnumerable<Depreciation> depreciations = await _depreciationRepository.GetDepreciationsAsync(tenantId);

            List<DepreciationDTO> depreciationDTOs = new();

            Parallel.ForEach(depreciations, depreciation =>
            {
                PeriodDTO? periodDTO = periodDTOs.FirstOrDefault(p => p.Id == depreciation.PeriodId);
                DepreciationDTO depreciationDTO = _mapper.Map<DepreciationDTO>(depreciation);

                if (periodDTO is not null)
                {
                    depreciationDTO.Year = periodDTO.Year;
                    depreciationDTO.Month = periodDTO.Month;
                }

                lock (depreciationDTOs)
                {
                    depreciationDTOs.Add(depreciationDTO);
                }
            });

            return depreciationDTOs;
        }

        public async Task<DepreciationDTO> GetDepreciationByIdAsync(Guid tenantId, Guid depreciationId)
        {
            Depreciation? depreciation = await _depreciationRepository.GetDepreciationByIdAsync(tenantId, depreciationId);

            if (depreciation is null) throw new NotFoundException("Depreciation");

            return _mapper.Map<DepreciationDTO>(depreciation);
        }

        public async Task<DepreciationDTO> UpdateDepreciationAsync(Guid tenantId, Guid depreciationId, UpdateDepreciationDTO updateDepreciationDTO, string userName)
        {
            // validation
            await _updateDepreciationDTOValidator.ValidateAndThrowAsync(updateDepreciationDTO);

            // check if exists
            await GetDepreciationByIdAsync(tenantId, depreciationId);

            Depreciation depreciation = new()
            {
                Id = depreciationId,
                DepreciationAmount = updateDepreciationDTO.DepreciationAmount,
                LastModificationAt = DateTime.Now,
                LastModificationBy = userName
            };
            depreciation = await _depreciationRepository.UpdateDepreciationAsync(depreciation);
            DepreciationDTO depreciationDTO = _mapper.Map<DepreciationDTO>(depreciation);

            return await MapDepreciationPeriodData(tenantId, depreciationDTO);
        }

        private async Task<DepreciationDTO> MapDepreciationPeriodData(Guid tenantId, DepreciationDTO depreciationDTO)
        {
            PeriodDTO periodDTO = await _periodService.GetPeriodByIdAsync(tenantId, depreciationDTO.PeriodId);
            depreciationDTO.Year = periodDTO.Year;
            depreciationDTO.Month = periodDTO.Month;
            return depreciationDTO;
        }

        public async Task<IEnumerable<FixedAssetYearDetailsDTO>> GetFixedAssetsYearDetailsAsync(Guid tenantId, int year, bool includeDeleted = false,  int? page = null, int? pageSize = null)
        {
            List<FixedAssetYearDetailsDTO> fixedAssetYearDetailsDTOs = new();

            DateTime firstDayOfYear = new(year, 1, 1);
            DateTime lastDayOfYear = new(year, 12, 31);

            IEnumerable<FixedAssetDTO> fixedAssetsDTOs = await _fixedAssetService.GetFixedAssetsAsync(tenantId, includeDeleted, page, pageSize);

            IEnumerable<DepreciationDTO> depreciationDTOs = await GetDepreciationsAsync(tenantId);

            List<DateTime> serviceDateTimes = await _arInvoiceService.GetListOfServiceDatesInPeriodAsync(tenantId, firstDayOfYear, lastDayOfYear);

            List<FixedAssetYearDetailsDTO> yearDetails = fixedAssetsDTOs
                .Where(f => f.CapitalizationDate <= lastDayOfYear)
                .AsParallel()
                .Select(fixedAssetDTO =>
                {
                    FixedAssetYearDetailsDTO fixedAssetYearDetailsDTO = _mapper.Map<FixedAssetYearDetailsDTO>(fixedAssetDTO);

                    IEnumerable<DepreciationDTO> fixedAssetDepreciationDTOs = depreciationDTOs.Where(d => d.FixedAssetId == fixedAssetDTO.Id);

                    decimal depreciationBeginningYear = fixedAssetDepreciationDTOs.Where(d => d.Year < year).Sum(d => d.DepreciationAmount);
                    decimal depreciationYear = fixedAssetDepreciationDTOs.Where(d => d.Year == year).Sum(d => d.DepreciationAmount);
                    decimal depreciationEndOfYear = depreciationBeginningYear + depreciationYear;

                    fixedAssetYearDetailsDTO.Year = year;
                    fixedAssetYearDetailsDTO.DepreciationAtYear = depreciationYear;
                    fixedAssetYearDetailsDTO.DepreciationBeginningYear = depreciationBeginningYear;
                    fixedAssetYearDetailsDTO.DepreciationEndOfYear = depreciationEndOfYear;
                    fixedAssetYearDetailsDTO.NetBookValueBeginningYear = fixedAssetDTO.AcquisitionAndProductionCosts - depreciationBeginningYear;
                    fixedAssetYearDetailsDTO.NetBookValueEndOfYear = fixedAssetDTO.AcquisitionAndProductionCosts - depreciationEndOfYear;

                    return fixedAssetYearDetailsDTO;
                })
                .ToList();

            fixedAssetYearDetailsDTOs.AddRange(yearDetails);

            IEnumerable<FixedAssetYearDetailsDTO> enumFixedAssetYearDetailsDTOs = fixedAssetYearDetailsDTOs.AsEnumerable();

            Pagination.Paginate(ref enumFixedAssetYearDetailsDTOs, page, pageSize);

            return enumFixedAssetYearDetailsDTOs;
        }

        public async Task<IEnumerable<DepreciationDTO>> GenerateDepreciationsAsync(Guid tenantId, Guid periodId, string userName)
        {
            PeriodDTO periodDTO = await _periodService.GetPeriodByIdAsync(tenantId, periodId);
            DateTime firstDayOfPeriod = new(periodDTO.Year, periodDTO.Month, 1);
            DateTime lastDayOfPeriod = new(periodDTO.Year, periodDTO.Month, DateTime.DaysInMonth(periodDTO.Year, periodDTO.Month));

            IEnumerable<DepreciationDTO> depreciationDTOs = await GetDepreciationsAsync(periodDTO.TenantId);

            IEnumerable<FixedAssetDTO> fixedAssetDTOs = await _fixedAssetService.GetFixedAssetsAsync(tenantId);

            List<DateTime> serviceDateTimes = await _arInvoiceService.GetListOfServiceDatesInPeriodAsync(tenantId, firstDayOfPeriod, lastDayOfPeriod);
            if (serviceDateTimes.Count == 0)
            {
                return Enumerable.Empty<DepreciationDTO>();
            }

            List<Task<DepreciationDTO>> depreciationTasks = fixedAssetDTOs
                .Where(f => f.CapitalizationDate <= lastDayOfPeriod)
                .Select(async fixedAssetDTO =>
                {
                    int nbOfDepreciationDays = serviceDateTimes.Count(s => s >= fixedAssetDTO.CapitalizationDate);
                    decimal depreciationPerDay = (fixedAssetDTO.DepreciationPercentagePerYear / 100) * (fixedAssetDTO.AcquisitionAndProductionCosts / 365);
                    decimal depreciationInPeriod = nbOfDepreciationDays * depreciationPerDay;

                    // check if depreciation exists
                    DepreciationDTO? depreciationDTO = depreciationDTOs.FirstOrDefault(d => d.FixedAssetId == fixedAssetDTO.Id && d.PeriodId == periodId);
                    if (depreciationDTO is not null)
                    {
                        // update depreciation
                        UpdateDepreciationDTO updateDepreciationDTO = new()
                        {
                            DepreciationAmount = depreciationInPeriod
                        };
                        return await UpdateDepreciationAsync(tenantId, depreciationDTO.Id, updateDepreciationDTO, userName);
                    }
                    else
                    {
                        // update depreciation
                        CreateDepreciationDTO createDepreciationDTO = new()
                        {
                            DepreciationAmount = depreciationInPeriod
                        };
                        return await CreateDepreciationAsync(tenantId, createDepreciationDTO, fixedAssetDTO.Id, periodId, userName);
                    }
                })
                .ToList();

            IEnumerable<DepreciationDTO> resultDepreciationDTOs = await Task.WhenAll(depreciationTasks);
            return resultDepreciationDTOs;
        }
    }
}