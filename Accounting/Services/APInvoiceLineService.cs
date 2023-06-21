using AccountingAPI.DTOs;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;

namespace AccountingAPI.Services
{
    public class APInvoiceLineService : IAPInvoiceLineService
    {
        private readonly IAPInvoiceLineRepository _invoiceLineRepository;
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IMapper _mapper;
        private readonly ILogger<APInvoiceLineService> _logger;

        public APInvoiceLineService(IAPInvoiceLineRepository invoiceLineRepository, ILogger<APInvoiceLineService> logger, IMapper mapper, IFixedAssetService fixedAssetService, IExpenseCategoryService expenseCategoryService)
        {
            _invoiceLineRepository = invoiceLineRepository;
            _logger = logger;
            _mapper = mapper;
            _fixedAssetService = fixedAssetService;
            _expenseCategoryService = expenseCategoryService;
        }

        public async Task<APInvoiceLineDTO> CreateAPInvoiceLineAsync(CreateAPInvoiceLineDTO createInvoiceLineDTO, Guid invoiceId, DateTime invoiceDate, string userName)
        {
            APInvoiceLineDTO invoiceLineDTO = new();

            APInvoiceLine invoiceLine = _mapper.Map<APInvoiceLine>(createInvoiceLineDTO);
            invoiceLine.TotalPrice = createInvoiceLineDTO.UnitPrice * createInvoiceLineDTO.Quantity;
            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.CreatedBy = userName;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.InsertAPInvoiceLineAsync(invoiceLine);

            ExpenseCategoryDTO? expenseCategoryDTO = await _expenseCategoryService.GetExpenseCategoryByIdAsync(invoiceLine.ExpenseCategoryId);

            if (expenseCategoryDTO == null) throw new Exception("Expense category does not exist");

            if (expenseCategoryDTO.ExpenseTypeCode == "Asset")
            {
                // create fixed asset
                CreateFixedAssetDTO createFixedAssetDTO = new()
                {
                    InvoiceLineId = invoiceLine.Id,
                    Description = invoiceLine.Description,
                    CapitalizationDate = invoiceDate,
                    AcquisitionAndProductionCosts = invoiceLine.TotalPrice,
                    DepreciationPercentagePerYear = createInvoiceLineDTO.DepreciationRatePerYear
                };
                FixedAssetDTO fixedAssetDTO = await _fixedAssetService.CreateFixedAssetAsync(createFixedAssetDTO, userName);

                invoiceLineDTO.FixedAsset = fixedAssetDTO;
            }

            return invoiceLineDTO;
        }
        public async Task<IEnumerable<APInvoiceLineDTO>> GetAPInvoiceLinesAsync(bool includeDeleted = false)
        {
            List<APInvoiceLineDTO> aPInvoiceLineDTOs = new();
            IEnumerable<APInvoiceLine> invoiceLines = await _invoiceLineRepository.GetAPInvoiceLinesAsync(includeDeleted);
            foreach (APInvoiceLine aPInvoiceLine in invoiceLines)
            {
                APInvoiceLineDTO aPInvoiceLineDTO = _mapper.Map<APInvoiceLineDTO>(aPInvoiceLine);
                aPInvoiceLineDTO.ExpenseCategory = await _expenseCategoryService.GetExpenseCategoryByIdAsync(aPInvoiceLine.ExpenseCategoryId);
                if (aPInvoiceLine.FixedAssetId != null) aPInvoiceLineDTO.FixedAsset = await _fixedAssetService.GetFixedAssetByIdAsync((Guid)aPInvoiceLine.FixedAssetId);
                aPInvoiceLineDTOs.Add(aPInvoiceLineDTO);
            }
            return aPInvoiceLineDTOs;
        }

        public async Task<APInvoiceLineDTO> GetAPInvoiceLineByIdAsync(Guid InvoiceLineId)
        {
            APInvoiceLine aPInvoiceLine = await _invoiceLineRepository.GetAPInvoiceLineByIdAsync(InvoiceLineId);
            APInvoiceLineDTO aPInvoiceLineDTO = _mapper.Map<APInvoiceLineDTO>(aPInvoiceLine);
            aPInvoiceLineDTO.ExpenseCategory = await _expenseCategoryService.GetExpenseCategoryByIdAsync(aPInvoiceLine.ExpenseCategoryId);
            if (aPInvoiceLine.FixedAssetId != null) aPInvoiceLineDTO.FixedAsset = await _fixedAssetService.GetFixedAssetByIdAsync((Guid)aPInvoiceLine.FixedAssetId);
            return aPInvoiceLineDTO;
        }

        public async Task<APInvoiceLineDTO> UpdateAPInvoiceLineAsync(UpdateAPInvoiceLineDTO udpateInvoiceLineDTO, string userName, Guid invoiceLineId, DateTime invoiceDate, Guid? fixedAssetId = null)
        {
            APInvoiceLine invoiceLine = _mapper.Map<APInvoiceLine>(udpateInvoiceLineDTO);
            invoiceLine.FixedAssetId = fixedAssetId;
            invoiceLine.Id = invoiceLineId;
            invoiceLine.LastModificationAt = invoiceDate;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.UpdateAPInvoiceLineAsync(invoiceLine);

            APInvoiceLineDTO aPInvoiceLineDTO = _mapper.Map<APInvoiceLineDTO>(invoiceLine);

            FixedAssetDTO? fixedAssetDTO = null;

            if (udpateInvoiceLineDTO.ExpenseCategoryType == "Asset")
            {
                // check if fixed asset already exists
                if (invoiceLine.FixedAssetId != null)
                {
                    // update fixed asset
                    UpdateFixedAssetDTO updateFixedAssetDTO = new()
                    {
                        Description = invoiceLine.Description,
                        CapitalizationDate = DateTime.Now,
                        AcquisitionAndProductionCosts = invoiceLine.TotalPrice,
                        DepreciationPercentagePerYear = udpateInvoiceLineDTO.DepreciationRatePerYear
                    };
                    fixedAssetDTO = await _fixedAssetService.UpdateFixedAssetAsync(updateFixedAssetDTO, userName, (Guid)invoiceLine.FixedAssetId);
                }
                else
                {
                    // add fixed asset
                    CreateFixedAssetDTO createFixedAssetDTO = new()
                    {
                        Description = invoiceLine.Description,
                        CapitalizationDate = DateTime.Now,
                        AcquisitionAndProductionCosts = invoiceLine.TotalPrice,
                        DepreciationPercentagePerYear = udpateInvoiceLineDTO.DepreciationRatePerYear
                    };
                    fixedAssetDTO = await _fixedAssetService.CreateFixedAssetAsync(createFixedAssetDTO, userName);
                }
            }
            else
            {
                if (invoiceLine.FixedAssetId != null)
                {
                    // delete fixed asset
                    await _fixedAssetService.SetDeletedFixedAssetAsync((Guid)invoiceLine.FixedAssetId, true);
                }
            }
            aPInvoiceLineDTO.FixedAsset = fixedAssetDTO;

            return aPInvoiceLineDTO;
        }

        public async Task<int> SetDeletedAPInvoiceLineAsync(Guid invoiceLineId, bool deleted)
        {
            return await _invoiceLineRepository.SetDeletedAPInvoiceLineAsync(invoiceLineId, deleted);
        }
    }
}