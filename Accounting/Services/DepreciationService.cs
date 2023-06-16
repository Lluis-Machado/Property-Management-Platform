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

            foreach(Depreciation depreciation in depreciations)
            {
                PeriodDTO? periodDTO = periodDTOs.FirstOrDefault(p => p.Id == depreciation.PeriodId);
                DepreciationDTO depreciationDTO = _mapper.Map<DepreciationDTO>(depreciation);

                if (periodDTO != null)
                {
                    depreciationDTO.Year = periodDTO.Year;
                    depreciationDTO.Month = periodDTO.Month;
                }

                depreciationDTOs.Add(depreciationDTO);
            }
            return depreciationDTOs;
        }

        public async Task<IEnumerable<FixedAssetYearDetailsDTO>> GetFixedAssetsYearDetailsAsync(Guid tenantId)
        {
            List<FixedAssetYearDetailsDTO> fixedAssetYearDetailsDTOs = new();

            IEnumerable<FixedAssetDTO> fixedAssetsDTOs = await _fixedAssetService.GetFixedAssetsAsync();

            IEnumerable<int> years = fixedAssetsDTOs.Select(f => f.CapitalizationDate.Year).Distinct();

            foreach (int year in years)
            {
                DateTime firstDayOfYear = new(year, 1, 1);
                DateTime lastDayOfYear = new(year, 12, 31);

                IEnumerable<DepreciationDTO> depreciationDTOs = await GetDepreciationsAsync(tenantId);

                depreciationDTOs = depreciationDTOs.Where(d => d.Year == year);

                List<DateTime> serviceDateTimes = await _arInvoiceLineService.GetListOfServiceDatesInPeriodAsync(firstDayOfYear, lastDayOfYear);

                foreach (FixedAssetDTO fixedAssetDTO in fixedAssetsDTOs.Where(f => f.CapitalizationDate >= firstDayOfYear))
                {
                    FixedAssetYearDetailsDTO fixedAssetYearDetailsDTO = _mapper.Map<FixedAssetYearDetailsDTO>(fixedAssetDTO);

                    IEnumerable<DepreciationDTO> fixedAssetdepreciationDTOs = depreciationDTOs.Where(d => d.FixedAssetId == fixedAssetDTO.Id);

                    double depreciationBeginningYear = depreciationDTOs.Where(d => d.Year < year).Sum(d => d.DepreciationAmount);
                    double depreciationYear = depreciationDTOs.Where(d => d.Year == year).Sum(d => d.DepreciationAmount);
                    double depreciationEndOfYear = depreciationBeginningYear + depreciationYear;

                    fixedAssetYearDetailsDTO.Year = year;
                    fixedAssetYearDetailsDTO.DepreciationBeginningYear = depreciationBeginningYear;
                    fixedAssetYearDetailsDTO.DepreciationEndOfYear = depreciationEndOfYear;
                    fixedAssetYearDetailsDTO.NetBookValueBeginningYear = fixedAssetDTO.AcquisitionAndProductionCosts - depreciationBeginningYear;
                    fixedAssetYearDetailsDTO.NetBookValueEndOfYear = fixedAssetDTO.AcquisitionAndProductionCosts - depreciationEndOfYear;
                    fixedAssetYearDetailsDTOs.Add(fixedAssetYearDetailsDTO);
                }
            }
            return fixedAssetYearDetailsDTOs;
        }

        public async Task<IEnumerable<DepreciationDTO>> SavePeriodDepreciationsAsync(Guid periodId, string userName)
        {
            List<DepreciationDTO> depreciationDTOs = new();

            PeriodDTO periodDTO = await _periodService.GetPeriodByIdAsync(periodId);

            DateTime firstDayOfPeriod = new(periodDTO.Year, periodDTO.Month, 1);
            DateTime lastDayOfPeriod = new(periodDTO.Year, periodDTO.Month, 31);

            IEnumerable<FixedAssetDTO> fixedAssetDTOs = await _fixedAssetService.GetFixedAssetsAsync();

            // Get list of service dates from ar invoices in period
            List<DateTime> serviceDateTimes = await _arInvoiceLineService.GetListOfServiceDatesInPeriodAsync(firstDayOfPeriod, lastDayOfPeriod);

            foreach (FixedAssetDTO fixedAssetDTO in fixedAssetDTOs.Where(f => f.CapitalizationDate <= lastDayOfPeriod))
            {
                // claculate number of depreciation days 
                int nbOfDepreciationDays = serviceDateTimes.Count(s => s >= fixedAssetDTO.CapitalizationDate);

                // calculate depreciation per day
                double depreciationPerDay = (fixedAssetDTO.DepreciationPercentagePerYear / 100) * (fixedAssetDTO.AcquisitionAndProductionCosts / 365);

                // calculate depreciation
                double depreciationInPeriod = nbOfDepreciationDays * depreciationPerDay;

                // insert depreciation 
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

                depreciationDTOs.Add(depreciationDTO);
            }
            return depreciationDTOs;
        }
    }
}