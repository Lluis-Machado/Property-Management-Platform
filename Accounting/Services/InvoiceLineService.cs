using AutoMapper;
using AccountingAPI.Repositories;
using AccountingAPI.DTOs;
using AccountingAPI.Models;

namespace AccountingAPI.Services
{
    public class InvoiceLineService : IInvoiceLineService
    {
        private readonly IInvoiceLineRepository _invoiceLineRepository;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceLineService> _logger;

        public InvoiceLineService(IInvoiceLineRepository invoiceLineRepository, ILogger<InvoiceLineService> logger, IMapper mapper, IFixedAssetService fixedAssetService)
        {
            _invoiceLineRepository = invoiceLineRepository;
            _logger = logger;
            _mapper = mapper;
            _fixedAssetService = fixedAssetService;
        }

        public async Task<InvoiceLineDTO> CreateInvoiceLineAsync(CreateInvoiceLineDTO createInvoiceLineDTO,Guid invoiceId, string userName)
        {
            InvoiceLineDTO invoiceLineDTO = new();

            InvoiceLine invoiceLine = _mapper.Map<InvoiceLine>(createInvoiceLineDTO);
            invoiceLine.TotalPrice = createInvoiceLineDTO.UnitPrice * createInvoiceLineDTO.Quantity;
            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.CreatedBy = userName;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.InsertInvoiceLineAsync(invoiceLine);

            if (createInvoiceLineDTO.ExpenseCategoryType == "Asset")
            {
                // create fixed asset
                CreateFixedAssetDTO createFixedAssetDTO = new()
                {
                    InvoiceLineId = invoiceLine.Id,
                    Description = invoiceLine.Description,
                    CapitalizationDate = DateTime.Now,
                    AcquisitionAndProductionCosts = invoiceLine.TotalPrice,
                    DepreciationPercentagePerYear = createInvoiceLineDTO.DepreciationRatePerYear
                };
                FixedAssetDTO fixedAssetDTO = await _fixedAssetService.CreateFixedAssetAsync(createFixedAssetDTO, userName);

                // update invoiceline with fixed asset id
                UpdateInvoiceLineDTO updateInvoiceLineDTO = _mapper.Map<UpdateInvoiceLineDTO>(invoiceLine);

                invoiceLineDTO = await UpdateInvoiceLineAsync(updateInvoiceLineDTO, userName, invoiceLine.Id, fixedAssetDTO.Id);
            }

            return invoiceLineDTO;  
        }
        public async Task<IEnumerable<InvoiceLineDTO>> GetInvoiceLinesAsync(bool includeDeleted = false)
        {
            IEnumerable<InvoiceLine> invoiceLines = await _invoiceLineRepository.GetInvoiceLinesAsync(includeDeleted);
            return _mapper.Map<IEnumerable<InvoiceLine>, List<InvoiceLineDTO>>(invoiceLines);
        }

        public async Task<InvoiceLineDTO> GetInvoiceLineByIdAsync(Guid InvoiceLineId)
        {
            InvoiceLine invoiceLine = await _invoiceLineRepository.GetInvoiceLineByIdAsync(InvoiceLineId);
            return _mapper.Map<InvoiceLineDTO>(invoiceLine);
        }

        public async Task<InvoiceLineDTO> UpdateInvoiceLineAsync(UpdateInvoiceLineDTO udpateInvoiceLineDTO, string userName, Guid invoiceLineId, Guid? fixedAssetId = null)
        {
            InvoiceLine invoiceLine = _mapper.Map<InvoiceLine>(udpateInvoiceLineDTO);
            invoiceLine.FixedAssetId = fixedAssetId;
            invoiceLine.Id = invoiceLineId;
            invoiceLine.LastModificationAt = DateTime.Now;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.UpdateInvoiceLineAsync(invoiceLine);

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
                    await _fixedAssetService.UpdateFixedAssetAsync(updateFixedAssetDTO, userName, (Guid)invoiceLine.FixedAssetId);
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
                    await _fixedAssetService.CreateFixedAssetAsync(createFixedAssetDTO, userName);
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

            return _mapper.Map<InvoiceLineDTO>(invoiceLine);
        }

        public async Task<int> SetDeletedInvoiceLineAsync(Guid invoiceLineId, bool deleted)
        {
            return await _invoiceLineRepository.SetDeletedInvoiceLineAsync(invoiceLineId,deleted);
        }
    }
}