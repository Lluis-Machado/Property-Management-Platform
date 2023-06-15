using AutoMapper;
using AccountingAPI.Repositories;
using AccountingAPI.DTOs;
using AccountingAPI.Models;

namespace AccountingAPI.Services
{
    public class ARInvoiceLineService : IARInvoiceLineService
    {
        private readonly IARInvoiceLineRepository _invoiceLineRepository;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IMapper _mapper;
        private readonly ILogger<ARInvoiceLineService> _logger;

        public ARInvoiceLineService(IARInvoiceLineRepository invoiceLineRepository, ILogger<ARInvoiceLineService> logger, IMapper mapper, IFixedAssetService fixedAssetService)
        {
            _invoiceLineRepository = invoiceLineRepository;
            _logger = logger;
            _mapper = mapper;
            _fixedAssetService = fixedAssetService;
        }

        public async Task<ARInvoiceLineDTO> CreateARInvoiceLineAsync(CreateARInvoiceLineDTO createInvoiceLineDTO,Guid invoiceId, string userName)
        {
            ARInvoiceLineDTO invoiceLineDTO = new();

            ARInvoiceLine invoiceLine = _mapper.Map<ARInvoiceLine>(createInvoiceLineDTO);
            invoiceLine.TotalPrice = createInvoiceLineDTO.UnitPrice * createInvoiceLineDTO.Quantity;
            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.CreatedBy = userName;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.InsertARInvoiceLineAsync(invoiceLine);

            return _mapper.Map<ARInvoiceLineDTO>(invoiceLine);  
        }
        public async Task<IEnumerable<ARInvoiceLineDTO>> GetARInvoiceLinesAsync(bool includeDeleted = false)
        {
            IEnumerable<InvoiceLine> invoiceLines = await _invoiceLineRepository.GetARInvoiceLinesAsync(includeDeleted);
            return _mapper.Map<IEnumerable<InvoiceLine>, List<ARInvoiceLineDTO>>(invoiceLines);
        }

        public async Task<ARInvoiceLineDTO> GetARInvoiceLineByIdAsync(Guid InvoiceLineId)
        {
            InvoiceLine invoiceLine = await _invoiceLineRepository.GetARInvoiceLineByIdAsync(InvoiceLineId);
            return _mapper.Map<ARInvoiceLineDTO>(invoiceLine);
        }

        public async Task<ARInvoiceLineDTO> UpdateARInvoiceLineAsync(UpdateARInvoiceLineDTO udpateInvoiceLineDTO, string userName, Guid invoiceLineId, Guid? fixedAssetId = null)
        {
            ARInvoiceLine invoiceLine = _mapper.Map<ARInvoiceLine>(udpateInvoiceLineDTO);
            invoiceLine.Id = invoiceLineId;
            invoiceLine.LastModificationAt = DateTime.Now;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.UpdateARInvoiceLineAsync(invoiceLine);

            return _mapper.Map<ARInvoiceLineDTO>(invoiceLine);
        }

        public async Task<int> SetDeletedARInvoiceLineAsync(Guid invoiceLineId, bool deleted)
        {
            return await _invoiceLineRepository.SetDeletedARInvoiceLineAsync(invoiceLineId,deleted);
        }
    }
}