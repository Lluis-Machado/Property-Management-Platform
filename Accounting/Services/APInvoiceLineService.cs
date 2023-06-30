using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class APInvoiceLineService : IAPInvoiceLineService
    {
        private readonly IAPInvoiceLineRepository _invoiceLineRepository;
        private readonly IValidator<CreateAPInvoiceLineDTO> _createAPInvoiceLineDTOValidator;
        private readonly IValidator<UpdateAPInvoiceLineDTO> _updateAPInvoiceLineDTOValidator;
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IMapper _mapper;
        private readonly ILogger<APInvoiceLineService> _logger;

        public APInvoiceLineService(IAPInvoiceLineRepository invoiceLineRepository, ILogger<APInvoiceLineService> logger, IMapper mapper, IFixedAssetService fixedAssetService, IExpenseCategoryService expenseCategoryService, IValidator<CreateAPInvoiceLineDTO> createAPInvoiceLineDTOValidator, IValidator<UpdateAPInvoiceLineDTO> updateAPInvoiceLineDTOValidator)
        {
            _invoiceLineRepository = invoiceLineRepository;
            _createAPInvoiceLineDTOValidator = createAPInvoiceLineDTOValidator;
            _updateAPInvoiceLineDTOValidator = updateAPInvoiceLineDTOValidator;
            _logger = logger;
            _mapper = mapper;
            _fixedAssetService = fixedAssetService;
            _expenseCategoryService = expenseCategoryService;
        }

        public async Task<APInvoiceLineDTO> CreateAPInvoiceLineAsync(Guid tenantId, Guid invoiceId, CreateAPInvoiceLineDTO createInvoiceLineDTO, DateTime invoiceDate, string userName)
        {
            // validation
            await _createAPInvoiceLineDTOValidator.ValidateAndThrowAsync(createInvoiceLineDTO);

            // check if expense category exists
            ExpenseCategoryDTO expenseCategoryDTO = await _expenseCategoryService.GetExpenseCategoryByIdAsync(createInvoiceLineDTO.ExpenseCategoryId);

            APInvoiceLine invoiceLine = _mapper.Map<APInvoiceLine>(createInvoiceLineDTO);
            invoiceLine.TotalPrice = createInvoiceLineDTO.UnitPrice * createInvoiceLineDTO.Quantity;
            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.CreatedBy = userName;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.InsertAPInvoiceLineAsync(invoiceLine);

            APInvoiceLineDTO invoiceLineDTO = _mapper.Map<APInvoiceLineDTO>(invoiceLine);

            invoiceLineDTO.ExpenseCategory = _mapper.Map<BasicExpenseCategoryDTO>(expenseCategoryDTO);

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
                FixedAssetDTO fixedAssetDTO = await _fixedAssetService.CreateFixedAssetAsync(tenantId, createFixedAssetDTO, userName);

                invoiceLineDTO.FixedAsset = fixedAssetDTO;
            }
            return invoiceLineDTO;
        }

        public async Task<IEnumerable<APInvoiceLineDTO>> GetAPInvoiceLinesAsync(Guid tenantId, bool includeDeleted = false)
        {
            List<APInvoiceLineDTO> aPInvoiceLineDTOs = new();

            IEnumerable<APInvoiceLine> invoiceLines = await _invoiceLineRepository.GetAPInvoiceLinesAsync(tenantId, includeDeleted);
            IEnumerable<ExpenseCategoryDTO> expenseCategoryDTOs = await _expenseCategoryService.GetExpenseCategoriesAsync(true);
            IEnumerable<FixedAssetDTO> fixedAssetDTOs = await _fixedAssetService.GetFixedAssetsAsync(tenantId,true);

            Parallel.ForEach(invoiceLines, aPInvoiceLine =>
            {
                APInvoiceLineDTO aPInvoiceLineDTO = _mapper.Map<APInvoiceLineDTO>(aPInvoiceLine);
                ExpenseCategoryDTO expenseCategoryDTO = expenseCategoryDTOs.First(e => e.Id == aPInvoiceLine.ExpenseCategoryId);
                aPInvoiceLineDTO.ExpenseCategory = _mapper.Map<BasicExpenseCategoryDTO>(expenseCategoryDTO);
                if (aPInvoiceLine.FixedAssetId is not null)
                    aPInvoiceLineDTO.FixedAsset = fixedAssetDTOs.First(f => f.InvoiceLineId == aPInvoiceLine.Id);
                aPInvoiceLineDTOs.Add(aPInvoiceLineDTO);
            });

            return aPInvoiceLineDTOs;
        }

        public async Task<APInvoiceLineDTO> GetAPInvoiceLineByIdAsync(Guid tenantId, Guid InvoiceLineId)
        {
            APInvoiceLine? aPInvoiceLine = await _invoiceLineRepository.GetAPInvoiceLineByIdAsync(tenantId, InvoiceLineId);

            if (aPInvoiceLine is null) throw new NotFoundException("AP Invoice line");

            APInvoiceLineDTO aPInvoiceLineDTO = _mapper.Map<APInvoiceLineDTO>(aPInvoiceLine);

            ExpenseCategoryDTO expenseCategoryDTO = await _expenseCategoryService.GetExpenseCategoryByIdAsync(aPInvoiceLine.ExpenseCategoryId);

            aPInvoiceLineDTO.ExpenseCategory = _mapper.Map<BasicExpenseCategoryDTO>(expenseCategoryDTO);

            if (aPInvoiceLine.FixedAssetId is not null)
            {
                aPInvoiceLineDTO.FixedAsset = await _fixedAssetService.GetFixedAssetByIdAsync(tenantId, (Guid)aPInvoiceLine.FixedAssetId);
            }

            return aPInvoiceLineDTO;
        }

        public async Task<APInvoiceLineDTO> UpdateAPInvoiceLineAsync(Guid tenantId, Guid invoiceLineId, UpdateAPInvoiceLineDTO udpateInvoiceLineDTO, string userName, DateTime invoiceDate, Guid? fixedAssetId = null)
        {
            // validation
            await _updateAPInvoiceLineDTOValidator.ValidateAndThrowAsync(udpateInvoiceLineDTO);

            // check if exists
            await GetAPInvoiceLineByIdAsync(tenantId, invoiceLineId);

            APInvoiceLine invoiceLine = _mapper.Map<APInvoiceLine>(udpateInvoiceLineDTO);
            invoiceLine.FixedAssetId = fixedAssetId;
            invoiceLine.Id = invoiceLineId;
            invoiceLine.LastModificationAt = invoiceDate;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.UpdateAPInvoiceLineAsync(invoiceLine);

            APInvoiceLineDTO aPInvoiceLineDTO = _mapper.Map<APInvoiceLineDTO>(invoiceLine);

            if (udpateInvoiceLineDTO.ExpenseCategoryType == "Asset")
            {
                // check if fixed asset already exists
                if (invoiceLine.FixedAssetId is not null)
                {
                    // update fixed asset
                    UpdateFixedAssetDTO updateFixedAssetDTO = new()
                    {
                        Description = invoiceLine.Description,
                        CapitalizationDate = DateTime.Now,
                        AcquisitionAndProductionCosts = invoiceLine.TotalPrice,
                        DepreciationPercentagePerYear = udpateInvoiceLineDTO.DepreciationRatePerYear
                    };
                    aPInvoiceLineDTO.FixedAsset = await _fixedAssetService.UpdateFixedAssetAsync(tenantId, (Guid)invoiceLine.FixedAssetId, updateFixedAssetDTO, userName);
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
                    aPInvoiceLineDTO.FixedAsset = await _fixedAssetService.CreateFixedAssetAsync(tenantId, createFixedAssetDTO, userName);
                }
            }
            else
            {
                if (invoiceLine.FixedAssetId is not null)
                {
                    // delete fixed asset
                    await _fixedAssetService.SetDeletedFixedAssetAsync(tenantId, (Guid)invoiceLine.FixedAssetId, true, userName);
                }
            }

            return aPInvoiceLineDTO;
        }

        public async Task SetDeletedAPInvoiceLineAsync(Guid tenantId, Guid invoiceLineId, bool deleted, string userName)
        {
            // check if exists
            APInvoiceLineDTO aPInvoiceLineDTO = await GetAPInvoiceLineByIdAsync(tenantId, invoiceLineId);

            // check if already deleted/undeleted
            if (aPInvoiceLineDTO.Deleted == deleted)
            {
                string action = deleted ? "deleted" : "undeleted";
                throw new ConflictException($"Invoice line already {action}");
            }

            await _invoiceLineRepository.SetDeletedAPInvoiceLineAsync(invoiceLineId, deleted, userName);
        }
    }
}