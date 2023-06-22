using AutoMapper;
using AccountingAPI.Repositories;
using AccountingAPI.DTOs;
using AccountingAPI.Models;
using System.Net;

namespace AccountingAPI.Services
{
    public class ARInvoiceLineService : IARInvoiceLineService
    {
        private readonly IARInvoiceLineRepository _invoiceLineRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ARInvoiceLineService> _logger;

        public ARInvoiceLineService(IARInvoiceLineRepository invoiceLineRepository, ILogger<ARInvoiceLineService> logger, IMapper mapper)
        {
            _invoiceLineRepository = invoiceLineRepository;
            _logger = logger;
            _mapper = mapper;
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
            ARInvoiceLine invoiceLine = await _invoiceLineRepository.GetARInvoiceLineByIdAsync(InvoiceLineId);
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

        public async Task<List<DateTime>> GetListOfServiceDatesInPeriodAsync(DateTime dateFrom, DateTime dateTo)
        {
            IEnumerable<ARInvoiceLine> arInvoiceLines = await _invoiceLineRepository.GetARInvoiceLinesAsync(false);

            List<DateTime> serviceDates = new();

            foreach (ARInvoiceLine arInvoiceLine in arInvoiceLines.Where(i => i.ServiceDateFrom > dateFrom || i.ServiceDateTo > dateTo))
            {
                if (arInvoiceLine.ServiceDateFrom == null || arInvoiceLine.ServiceDateTo == null) continue; 

                for (DateTime dt = (DateTime)arInvoiceLine.ServiceDateFrom; dt <= arInvoiceLine.ServiceDateTo; dt = dt.AddDays(1))
                {
                    if (!serviceDates.Contains(dt)) serviceDates.Add(dt);
                }
            }
            return serviceDates;
        }

        public async Task<int> SetDeletedARInvoiceLineAsync(Guid invoiceLineId, bool deleted)
        {
            return await _invoiceLineRepository.SetDeletedARInvoiceLineAsync(invoiceLineId,deleted);
        }
    }
}