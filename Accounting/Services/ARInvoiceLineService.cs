using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class ARInvoiceLineService : IARInvoiceLineService
    {
        private readonly IARInvoiceLineRepository _invoiceLineRepository;
        private readonly IValidator<CreateARInvoiceLineDTO> _createARInvoiceLineDTOValidator;
        private readonly IValidator<UpdateARInvoiceLineDTO> _updateARInvoiceLineDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ARInvoiceLineService> _logger;

        public ARInvoiceLineService(IARInvoiceLineRepository invoiceLineRepository, IValidator<CreateARInvoiceLineDTO> createARInvoiceLineDTOValidator, IValidator<UpdateARInvoiceLineDTO> updateARInvoiceLineDTOValidator, ILogger<ARInvoiceLineService> logger, IMapper mapper)
        {
            _invoiceLineRepository = invoiceLineRepository;
            _createARInvoiceLineDTOValidator = createARInvoiceLineDTOValidator;
            _updateARInvoiceLineDTOValidator = updateARInvoiceLineDTOValidator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ARInvoiceLineDTO> CreateARInvoiceLineAsync(Guid tenantId, Guid invoiceId, CreateARInvoiceLineDTO createInvoiceLineDTO, string userName)
        {
            // validation
            await _createARInvoiceLineDTOValidator.ValidateAndThrowAsync(createInvoiceLineDTO);

            ARInvoiceLine invoiceLine = _mapper.Map<ARInvoiceLine>(createInvoiceLineDTO);
            invoiceLine.TotalPrice = createInvoiceLineDTO.UnitPrice * createInvoiceLineDTO.Quantity;
            invoiceLine.InvoiceId = invoiceId;
            invoiceLine.CreatedBy = userName;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.InsertARInvoiceLineAsync(invoiceLine);

            return _mapper.Map<ARInvoiceLineDTO>(invoiceLine);
        }
        public async Task<IEnumerable<ARInvoiceLineDTO>> GetARInvoiceLinesAsync(Guid tenantId, bool includeDeleted = false)
        {
            IEnumerable<InvoiceLine> invoiceLines = await _invoiceLineRepository.GetARInvoiceLinesAsync(tenantId, includeDeleted);
            return _mapper.Map<IEnumerable<InvoiceLine>, List<ARInvoiceLineDTO>>(invoiceLines);
        }

        public async Task<ARInvoiceLineDTO> GetARInvoiceLineByIdAsync(Guid tenantId, Guid invoiceLineId)
        {
            ARInvoiceLine? invoiceLine = await _invoiceLineRepository.GetARInvoiceLineByIdAsync(tenantId, invoiceLineId);

            if (invoiceLine is null) throw new NotFoundException("AR Invoice line");

            return _mapper.Map<ARInvoiceLineDTO>(invoiceLine);
        }

        public async Task<ARInvoiceLineDTO> UpdateARInvoiceLineAsync(Guid tenantId, Guid invoiceLineId, UpdateARInvoiceLineDTO udpateInvoiceLineDTO, string userName, Guid? fixedAssetId = null)
        {
            // validation
            await _updateARInvoiceLineDTOValidator.ValidateAndThrowAsync(udpateInvoiceLineDTO);

            // check if exists
            await GetARInvoiceLineByIdAsync(tenantId, invoiceLineId);

            ARInvoiceLine invoiceLine = _mapper.Map<ARInvoiceLine>(udpateInvoiceLineDTO);
            invoiceLine.Id = invoiceLineId;
            invoiceLine.LastModificationAt = DateTime.Now;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.UpdateARInvoiceLineAsync(invoiceLine);

            return _mapper.Map<ARInvoiceLineDTO>(invoiceLine);
        }

        public async Task SetDeletedARInvoiceLineAsync(Guid tenantId, Guid invoiceLineId, bool deleted, string userName)
        {
            // check if exists
            ARInvoiceLineDTO aRInvoiceLineDTO = await GetARInvoiceLineByIdAsync(tenantId, invoiceLineId);

            // check if already deleted/undeleted
            if (aRInvoiceLineDTO.Deleted == deleted)
            {
                string action = deleted ? "deleted" : "undeleted";
                throw new ConflictException($"Invoice line already {action}");
            }

            await _invoiceLineRepository.SetDeletedARInvoiceLineAsync(invoiceLineId, deleted, userName);
        }

        public async Task<List<DateTime>> GetListOfServiceDatesInPeriodAsync(Guid tenantId, DateTime dateFrom, DateTime dateTo)
        {
            IEnumerable<ARInvoiceLine> arInvoiceLines = await _invoiceLineRepository.GetARInvoiceLinesAsync(tenantId, false);

            List<DateTime> serviceDates = new();

            await Task.Run(() =>
            {
                object lockObject = new();

                Parallel.ForEach(arInvoiceLines.Where(i => i.ServiceDateFrom > dateFrom || i.ServiceDateTo > dateTo), arInvoiceLine =>
                {
                    if (arInvoiceLine.ServiceDateFrom is null || arInvoiceLine.ServiceDateTo is null) return;

                    for (DateTime dt = (DateTime)arInvoiceLine.ServiceDateFrom; dt <= arInvoiceLine.ServiceDateTo; dt = dt.AddDays(1))
                    {
                        lock (lockObject)
                        {
                            if (!serviceDates.Contains(dt))
                            {
                                serviceDates.Add(dt);
                            }
                        }
                    }
                });
            });

            return serviceDates;
        }

    }
}