using AutoMapper;
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

        public async Task<IEnumerable<FixedAssetYearDetailsDTO>> GetFixedAssetsYearDetailsAsync(Guid tenantId)
        {
            IEnumerable<FixedAssetDTO> fixedAssetsDTOs = await _fixedAssetService.GetFixedAssetsAsync();

            List<FixedAssetYearDetailsDTO> fixedAssetYearDetailsDTOs = new();

            foreach (int year in fixedAssetsDTOs.Select(f => f.CapitalizationDate.Year).Distinct())
            {
                DateTime firstDayOfYear = new(year, 1, 1);
                DateTime lastDayOfYear = new(year, 12, 31);

                IEnumerable<DepreciationDTO> depreciationDTOs = await GetDepreciationsAsync(tenantId);
                depreciationDTOs = depreciationDTOs.Where(d => d.Year == year);

                List<DateTime> serviceDateTimes = await _arInvoiceLineService.GetListOfServiceDatesInPeriodAsync(firstDayOfYear, lastDayOfYear);

                List<FixedAssetYearDetailsDTO> yearDetails = fixedAssetsDTOs
                    .Where(f => f.CapitalizationDate >= firstDayOfYear)
                    .AsParallel()
                    .Select(fixedAssetDTO =>
                    {
                        FixedAssetYearDetailsDTO fixedAssetYearDetailsDTO = _mapper.Map<FixedAssetYearDetailsDTO>(fixedAssetDTO);

                        IEnumerable<DepreciationDTO> fixedAssetDepreciationDTOs = depreciationDTOs.Where(d => d.FixedAssetId == fixedAssetDTO.Id);

                        double depreciationBeginningYear = depreciationDTOs.Where(d => d.Year < year).Sum(d => d.DepreciationAmount);
                        double depreciationYear = depreciationDTOs.Where(d => d.Year == year).Sum(d => d.DepreciationAmount);
                        double depreciationEndOfYear = depreciationBeginningYear + depreciationYear;

                        fixedAssetYearDetailsDTO.Year = year;
                        fixedAssetYearDetailsDTO.DepreciationBeginningYear = depreciationBeginningYear;
                        fixedAssetYearDetailsDTO.DepreciationEndOfYear = depreciationEndOfYear;
                        fixedAssetYearDetailsDTO.NetBookValueBeginningYear = fixedAssetDTO.AcquisitionAndProductionCosts - depreciationBeginningYear;
                        fixedAssetYearDetailsDTO.NetBookValueEndOfYear = fixedAssetDTO.AcquisitionAndProductionCosts - depreciationEndOfYear;

                        return fixedAssetYearDetailsDTO;
                    })
                    .ToList();

                fixedAssetYearDetailsDTOs.AddRange(yearDetails);
            }

            return fixedAssetYearDetailsDTOs;
        }


        public async Task<IEnumerable<DepreciationDTO>> GenerateDepreciationsAsync(Guid periodId, string userName)
        {
            PeriodDTO periodDTO = await _periodService.GetPeriodByIdAsync(periodId);
            DateTime firstDayOfPeriod = new DateTime(periodDTO.Year, periodDTO.Month, 1);
            DateTime lastDayOfPeriod = new DateTime(periodDTO.Year, periodDTO.Month, DateTime.DaysInMonth(periodDTO.Year, periodDTO.Month));

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

                    Depreciation depreciation = new()
                    {
                        FixedAssetId = fixedAssetDTO.Id,
                        PeriodId = periodId,
                        DepreciationAmount = depreciationInPeriod,
                        CreatedBy = userName,
                        LastModificationBy = userName
                    };

                    depreciation = await _depreciationRepository.InsertDepreciationAsync(depreciation);
                    DepreciationDTO depreciationDTO = _mapper.Map<DepreciationDTO>(depreciation);
                    return depreciationDTO;
                })
                .ToList();

            IEnumerable<DepreciationDTO> depreciationDTOs = await Task.WhenAll(depreciationTasks);
            return depreciationDTOs;
        }


    }
}