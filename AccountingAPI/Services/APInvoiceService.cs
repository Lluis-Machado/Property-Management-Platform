using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AccountingAPI.Utilities;
using AutoMapper;
using FluentValidation;
using System.Transactions;

namespace AccountingAPI.Services
{
    public class APInvoiceService : IAPInvoiceService
    {
        private readonly IAPInvoiceRepository _invoiceRepository;
        private readonly IAPInvoiceLineRepository _invoiceLineRepository;
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IPeriodService _periodService;
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly IValidator<CreateAPInvoiceDTO> _createAPInvoiceDTOValidator;
        private readonly IValidator<UpdateAPInvoiceDTO> _updateAPInvoiceDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<APInvoiceService> _logger;

        public APInvoiceService(IAPInvoiceRepository invoiceRepository
            , IAPInvoiceLineRepository invoiceLineRepository
            , ILogger<APInvoiceService> logger
            , IMapper mapper
            , IValidator<CreateAPInvoiceDTO> createAPInvoiceDTOValidator
            , IValidator<UpdateAPInvoiceDTO> updateAPInvoiceDTOValidator
            , IBusinessPartnerService businessPartnerService
            , IPeriodService periodService
            , IFixedAssetService fixedAssetService
            , IExpenseCategoryService expenseCategoryService)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _logger = logger;
            _fixedAssetService = fixedAssetService;
            _expenseCategoryService = expenseCategoryService;
            _periodService = periodService;
            _mapper = mapper;
            _createAPInvoiceDTOValidator = createAPInvoiceDTOValidator;
            _updateAPInvoiceDTOValidator = updateAPInvoiceDTOValidator;
            _businessPartnerService = businessPartnerService;
        }

        public async Task<APInvoiceDTO> CreateAPInvoiceAndLinesAsync(Guid tenantId, Guid businessPartnerId, CreateAPInvoiceDTO createInvoiceDTO, string userName)
        {
            // validation
            await _createAPInvoiceDTOValidator.ValidateAndThrowAsync(createInvoiceDTO);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // insert invoice
                APInvoice invoice = _mapper.Map<APInvoice>(createInvoiceDTO);

                invoice.GrossAmount = createInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity);
                invoice.NetAmount = createInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity / (1 + invoiceLine.Tax));
                invoice.CreatedBy = userName;
                invoice.LastModificationBy = userName;
                invoice.BusinessPartnerId = businessPartnerId;

                invoice = await _invoiceRepository.InsertAPInvoice(invoice);

                APInvoiceDTO invoiceDTO = _mapper.Map<APInvoiceDTO>(invoice);
                BusinessPartnerDTO businessPartnerDTO = await _businessPartnerService.GetBusinessPartnerByIdAsync(tenantId, businessPartnerId);
                invoiceDTO.BusinessPartner = _mapper.Map<BasicBusinessPartnerDTO>(businessPartnerDTO);
                // insert invoice lines
                Parallel.ForEach(createInvoiceDTO.InvoiceLines, async (createInvoiceLineDTO) =>
                {
                    APInvoiceLineDTO invoiceLineDTO = await CreateAPInvoiceLineAsync(tenantId, invoiceDTO.Id, createInvoiceLineDTO, invoice.Date, userName);

                    lock (invoiceDTO.InvoiceLines) // Lock access to the shared collection
                    {
                        invoiceDTO.InvoiceLines.Add(invoiceLineDTO);
                    }
                });

                transaction.Complete();

                return invoiceDTO;
            }
        }

        public async Task<IEnumerable<APInvoiceDTO>> GetAPInvoicesAsync(Guid tenantId, bool includeDeleted = false, int? page = null, int? pageSize = null)
        {
            List<APInvoiceDTO> invoiceDTOs = new();
            IEnumerable<Invoice> invoices = await _invoiceRepository.GetAPInvoicesAsync(tenantId, includeDeleted);

            Pagination.Paginate(ref invoices, page, pageSize);

            IEnumerable<APInvoiceLineDTO> invoiceLinesDTOs = await GetAPInvoiceLinesAsync(tenantId, includeDeleted);
            IEnumerable<BusinessPartnerDTO> businessPartnerDTOs = await _businessPartnerService.GetBusinessPartnersAsync(tenantId, includeDeleted);
            Parallel.ForEach(invoices, (invoice) =>
            {
                APInvoiceDTO invoiceDTO = _mapper.Map<APInvoiceDTO>(invoice);
                invoiceDTO.InvoiceLines = invoiceLinesDTOs.Where(i => i.InvoiceId == invoice.Id).ToList();
                BusinessPartnerDTO businessPartnerDTO = businessPartnerDTOs.First(b => b.Id == invoice.BusinessPartnerId);
                invoiceDTO.BusinessPartner = _mapper.Map<BasicBusinessPartnerDTO>(businessPartnerDTO);
                lock (invoiceDTOs) // Lock access to the shared collection
                {
                    invoiceDTOs.Add(invoiceDTO);
                }
            });
            return invoiceDTOs;
        }

        public async Task<APInvoiceDTO> GetAPInvoiceByIdAsync(Guid tenantId, Guid invoiceId)
        {
            APInvoiceDTO invoiceDTO = new();
            IEnumerable<APInvoiceLineDTO> invoiceLines = await GetAPInvoiceLinesAsync(tenantId);
            Invoice? invoice = await _invoiceRepository.GetAPInvoiceByIdAsync(tenantId, invoiceId);

            if (invoice is null) throw new NotFoundException("AP Invoice not found");

            invoiceDTO = _mapper.Map<APInvoiceDTO>(invoice);
            invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();

            return invoiceDTO;
        }

        public async Task<APInvoiceDTO> UpdateAPInvoiceAndLinesAsync(Guid tenantId, Guid invoiceId, UpdateAPInvoiceDTO updateInvoiceDTO, string userName)
        {
            // validation
            await _updateAPInvoiceDTOValidator.ValidateAndThrowAsync(updateInvoiceDTO);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Get and Update Invoice
                APInvoiceDTO actualInvoiceDTO = await GetAPInvoiceByIdAsync(tenantId, invoiceId);

                APInvoice invoice = _mapper.Map<APInvoice>(actualInvoiceDTO);
                invoice.GrossAmount = updateInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity);
                invoice.NetAmount = updateInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity / (1 + invoiceLine.Tax));
                invoice.CreatedBy = userName;
                invoice.LastModificationBy = userName;

                invoice = await _invoiceRepository.UpdateAPInvoiceAsync(invoice);

                List<Guid> updateInvoiceLineIds = new();

                // check Add/Update invoice lines 
                foreach (UpdateAPInvoiceLineDTO updateInvoiceLineDTO in updateInvoiceDTO.InvoiceLines)
                {
                    if (updateInvoiceLineDTO.Id is not null)
                    {
                        // Update invoice line
                        APInvoiceLineDTO updatedInvoiceLine = await UpdateAPInvoiceLineAsync(tenantId, (Guid)updateInvoiceLineDTO.Id, updateInvoiceLineDTO, userName, invoice.Date);
                        updateInvoiceLineIds.Add(updatedInvoiceLine.Id);
                    }
                    else
                    {
                        // Add invoice line
                        CreateAPInvoiceLineDTO createInvoiceLineDTO = _mapper.Map<CreateAPInvoiceLineDTO>(updateInvoiceLineDTO);
                        await CreateAPInvoiceLineAsync(tenantId, actualInvoiceDTO.Id, createInvoiceLineDTO, invoice.Date, userName);
                    }
                }

                foreach (APInvoiceLineDTO actualInvoiceLineDTO in actualInvoiceDTO.InvoiceLines)
                {
                    if (!updateInvoiceLineIds.Contains(actualInvoiceLineDTO.Id))
                    {
                        // delete invoice line
                        await SetDeletedAPInvoiceLineAsync(actualInvoiceLineDTO.Id, true, userName);
                    }
                }

                APInvoiceDTO apInvoiceDTO = await GetAPInvoiceByIdAsync(tenantId, invoiceId);

                transaction.Complete();

                return apInvoiceDTO;
            }
        }

        public async Task SetDeletedAPInvoiceAsync(Guid tenantId, Guid invoiceId, bool deleted, string userName)
        {
            // check if exists
            APInvoiceDTO aPInvoiceDTO = await GetAPInvoiceByIdAsync(tenantId, invoiceId);

            // check if already deleted/undeleted
            if (aPInvoiceDTO.Deleted == deleted)
            {
                string action = deleted ? "deleted" : "undeleted";
                throw new ConflictException($"Invoice already {action}");
            }

            await _invoiceRepository.SetDeletedAPInvoiceAsync(invoiceId, deleted, userName);
        }

        public async Task<APInvoiceLineDTO> CreateAPInvoiceLineAsync(Guid tenantId, Guid invoiceId, CreateAPInvoiceLineDTO createInvoiceLineDTO, DateTime invoiceDate, string userName)
        {
            // check if expense category exists
            if (createInvoiceLineDTO.ExpenseCategoryId is null) throw new Exception("Expense category ID cannot be null");
            ExpenseCategoryDTO expenseCategoryDTO = await _expenseCategoryService.GetExpenseCategoryByIdAsync((Guid)createInvoiceLineDTO.ExpenseCategoryId);

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
            IEnumerable<FixedAssetDTO> fixedAssetDTOs = await _fixedAssetService.GetFixedAssetsAsync(tenantId, true);

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

        private async Task<APInvoiceLineDTO> GetAPInvoiceLineByIdAsync(Guid tenantId, Guid InvoiceLineId)
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

        private async Task<APInvoiceLineDTO> UpdateAPInvoiceLineAsync(Guid tenantId, Guid invoiceLineId, UpdateAPInvoiceLineDTO udpateInvoiceLineDTO, string userName, DateTime invoiceDate, Guid? fixedAssetId = null)
        {
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

        private async Task SetDeletedAPInvoiceLineAsync(Guid invoiceLineId, bool deleted, string userName)
        {
            await _invoiceLineRepository.SetDeletedAPInvoiceLineAsync(invoiceLineId, deleted, userName);
        }

    }
}