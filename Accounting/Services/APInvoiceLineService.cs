using AutoMapper;
using AccountingAPI.Repositories;
using AccountingAPI.DTOs;
using AccountingAPI.Models;

namespace AccountingAPI.Services
{
    public class APInvoiceLineService : IAPInvoiceLineService
    {
        private readonly IAPInvoiceLineRepository _invoiceLineRepository;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IMapper _mapper;
        private readonly ILogger<APInvoiceLineService> _logger;

        public APInvoiceLineService(IAPInvoiceLineRepository invoiceLineRepository, ILogger<APInvoiceLineService> logger, IMapper mapper, IFixedAssetService fixedAssetService)
        {
            _invoiceLineRepository = invoiceLineRepository;
            _logger = logger;
            _mapper = mapper;
            _fixedAssetService = fixedAssetService;
        }

        public async Task<APInvoiceLineDTO> CreateAPInvoiceLineAsync(CreateAPInvoiceLineDTO createInvoiceLineDTO,Guid invoiceId, string userName)
        {
            APInvoiceLineDTO invoiceLineDTO = new();

            APInvoiceLine invoiceLine = _mapper.Map<APInvoiceLine>(createInvoiceLineDTO);
            invoiceLine.TotalPrice = createInvoiceLineDTO.UnitPrice * createInvoiceLineDTO.Quantity;
            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.CreatedBy = userName;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.InsertAPInvoiceLineAsync(invoiceLine);

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
                UpdateAPInvoiceLineDTO updateInvoiceLineDTO = _mapper.Map<UpdateAPInvoiceLineDTO>(invoiceLine);

                invoiceLineDTO = await UpdateAPInvoiceLineAsync(updateInvoiceLineDTO, userName, invoiceLine.Id, fixedAssetDTO.Id);
            }

            return invoiceLineDTO;  
        }
        public async Task<IEnumerable<APInvoiceLineDTO>> GetAPInvoiceLinesAsync(bool includeDeleted = false)
        {
            IEnumerable<InvoiceLine> invoiceLines = await _invoiceLineRepository.GetAPInvoiceLinesAsync(includeDeleted);
            return _mapper.Map<IEnumerable<InvoiceLine>, List<APInvoiceLineDTO>>(invoiceLines);
        }

        public async Task<APInvoiceLineDTO> GetAPInvoiceLineByIdAsync(Guid InvoiceLineId)
        {
            InvoiceLine invoiceLine = await _invoiceLineRepository.GetAPInvoiceLineByIdAsync(InvoiceLineId);
            return _mapper.Map<APInvoiceLineDTO>(invoiceLine);
        }

        public async Task<APInvoiceLineDTO> UpdateAPInvoiceLineAsync(UpdateAPInvoiceLineDTO udpateInvoiceLineDTO, string userName, Guid invoiceLineId, Guid? fixedAssetId = null)
        {
            APInvoiceLine invoiceLine = _mapper.Map<APInvoiceLine>(udpateInvoiceLineDTO);
            invoiceLine.FixedAssetId = fixedAssetId;
            invoiceLine.Id = invoiceLineId;
            invoiceLine.LastModificationAt = DateTime.Now;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.UpdateAPInvoiceLineAsync(invoiceLine);

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

            return _mapper.Map<APInvoiceLineDTO>(invoiceLine);
        }

        public async Task<int> SetDeletedAPInvoiceLineAsync(Guid invoiceLineId, bool deleted)
        {
            return await _invoiceLineRepository.SetDeletedAPInvoiceLineAsync(invoiceLineId,deleted);
        }
    }
}