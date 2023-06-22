﻿using AutoMapper;
using AccountingAPI.Repositories;
using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public class DepreciationService : IDepreciationService
    {
        private readonly IDepreciationRepository _depreciationRepository;
        private readonly IPeriodService _periodService;
        private readonly IARInvoiceLineService _arInvoiceLineService;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IMapper _mapper;
        private readonly ILogger<DepreciationService> _logger;

        public DepreciationService(IDepreciationRepository depreciationRepository, ILogger<DepreciationService> logger, IMapper mapper, IFixedAssetService fixedAssetService, IARInvoiceLineService arInvoiceLineService, IPeriodService periodService)
        {
            _depreciationRepository = depreciationRepository;
            _periodService = periodService;
            _logger = logger;
            _mapper = mapper;
            _fixedAssetService = fixedAssetService;
            _arInvoiceLineService = arInvoiceLineService;
        }

        private async Task<DepreciationDTO> MapDepreciationPeriodData(DepreciationDTO depreciationDTO)
        {
            PeriodDTO periodDTO = await _periodService.GetPeriodByIdAsync(depreciationDTO.PeriodId);
            depreciationDTO.Year = periodDTO.Year;
            depreciationDTO.Month = periodDTO.Month;
            return depreciationDTO;
        }

            public async Task<IEnumerable<DepreciationDTO>> GetDepreciationsAsync(Guid tenantId)
        {
            IEnumerable<PeriodDTO> periodDTOs = await _periodService.GetPeriodsAsync(tenantId);
            IEnumerable<Depreciation> depreciations = await _depreciationRepository.GetDepreciationsAsync();

            List<DepreciationDTO> depreciationDTOs = new();

            Parallel.ForEach(depreciations, depreciation =>
            {
                PeriodDTO? periodDTO = periodDTOs.FirstOrDefault(p => p.Id == depreciation.PeriodId);
                DepreciationDTO depreciationDTO = _mapper.Map<DepreciationDTO>(depreciation);

                if (periodDTO != null)
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

        public async Task<IEnumerable<FixedAssetYearDetailsDTO>> GetFixedAssetsYearDetailsAsync(Guid tenantId, int year)
        {
            List<FixedAssetYearDetailsDTO> fixedAssetYearDetailsDTOs = new();

            DateTime firstDayOfYear = new(year, 1, 1);
            DateTime lastDayOfYear = new(year, 12, 31);

            IEnumerable<FixedAssetDTO> fixedAssetsDTOs = await _fixedAssetService.GetFixedAssetsAsync();

            IEnumerable<DepreciationDTO> depreciationDTOs = await GetDepreciationsAsync(tenantId);

            List<DateTime> serviceDateTimes = await _arInvoiceLineService.GetListOfServiceDatesInPeriodAsync(firstDayOfYear, lastDayOfYear);

            List<FixedAssetYearDetailsDTO> yearDetails = fixedAssetsDTOs
                .Where(f => f.CapitalizationDate <= lastDayOfYear)
                .AsParallel()
                .Select(fixedAssetDTO =>
                {
                    FixedAssetYearDetailsDTO fixedAssetYearDetailsDTO = _mapper.Map<FixedAssetYearDetailsDTO>(fixedAssetDTO);

                    IEnumerable<DepreciationDTO> fixedAssetDepreciationDTOs = depreciationDTOs.Where(d => d.FixedAssetId == fixedAssetDTO.Id);

                    double depreciationBeginningYear = fixedAssetDepreciationDTOs.Where(d => d.Year < year).Sum(d => d.DepreciationAmount);
                    double depreciationYear = fixedAssetDepreciationDTOs.Where(d => d.Year == year).Sum(d => d.DepreciationAmount);
                    double depreciationEndOfYear = depreciationBeginningYear + depreciationYear;

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
            return fixedAssetYearDetailsDTOs;
        }

        public async Task<IEnumerable<DepreciationDTO>> GenerateDepreciationsAsync(Guid periodId, string userName)
        {
            PeriodDTO periodDTO = await _periodService.GetPeriodByIdAsync(periodId);
            DateTime firstDayOfPeriod = new(periodDTO.Year, periodDTO.Month, 1);
            DateTime lastDayOfPeriod = new(periodDTO.Year, periodDTO.Month, DateTime.DaysInMonth(periodDTO.Year, periodDTO.Month));

            IEnumerable<DepreciationDTO> depreciationDTOs = await GetDepreciationsAsync(periodDTO.TenantId);

            IEnumerable<FixedAssetDTO> fixedAssetDTOs = await _fixedAssetService.GetFixedAssetsAsync();

            List<DateTime> serviceDateTimes = await _arInvoiceLineService.GetListOfServiceDatesInPeriodAsync(firstDayOfPeriod, lastDayOfPeriod);
            if (serviceDateTimes.Count == 0)
            {
                return Enumerable.Empty<DepreciationDTO>();
            }

            List<Task<DepreciationDTO>> depreciationTasks = fixedAssetDTOs
                .Where(f => f.CapitalizationDate <= lastDayOfPeriod)
                .Select(async fixedAssetDTO =>
                {
                    int nbOfDepreciationDays = serviceDateTimes.Count(s => s >= fixedAssetDTO.CapitalizationDate);
                    double depreciationPerDay = (fixedAssetDTO.DepreciationPercentagePerYear / 100) * (fixedAssetDTO.AcquisitionAndProductionCosts / 365);
                    double depreciationInPeriod = nbOfDepreciationDays * depreciationPerDay;

                    // check if depreciation exists
                    DepreciationDTO? depreciationDTO = depreciationDTOs.FirstOrDefault(d => d.FixedAssetId == fixedAssetDTO.Id && d.PeriodId == periodId);
                    if (depreciationDTO != null)
                    {
                        // update depreciation
                        UpdateDepreciationDTO updateDepreciationDTO = new()
                        {
                            DepreciationAmount = depreciationInPeriod
                        };
                       return await UpdateDepreciationAsync(depreciationDTO.Id, updateDepreciationDTO, userName);
                    }
                    else
                    {
                        // update depreciation
                        CreateDepreciationDTO createDepreciationDTO = new()
                        {
                            DepreciationAmount = depreciationInPeriod
                        };
                        return await CreateDepreciationAsync(createDepreciationDTO, fixedAssetDTO.Id, periodId, userName);
                    }
                })
                .ToList();

            IEnumerable<DepreciationDTO> resultDepreciationDTOs = await Task.WhenAll(depreciationTasks);
            return resultDepreciationDTOs;
        }

        public async Task<DepreciationDTO> CreateDepreciationAsync(CreateDepreciationDTO createDepreciationDTO, Guid fixedAssetId, Guid periodId, string userName)
        {
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
            return await MapDepreciationPeriodData(depreciationDTO);
        }

        public async Task<DepreciationDTO> UpdateDepreciationAsync(Guid depreciationId, UpdateDepreciationDTO updateDepreciationDTO, string userName)
        {
            Depreciation depreciation = new()
            {
                Id = depreciationId,
                DepreciationAmount = updateDepreciationDTO.DepreciationAmount,
                LastModificationAt = DateTime.Now,
                LastModificationBy = userName
            };
            depreciation = await _depreciationRepository.UpdateDepreciationAsync(depreciation);
            DepreciationDTO depreciationDTO = _mapper.Map<DepreciationDTO>(depreciation);
            return await MapDepreciationPeriodData(depreciationDTO);
        }


    }
}