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
    public class ARInvoiceService : IARInvoiceService
    {
        private readonly IARInvoiceRepository _invoiceRepository;
        private readonly IARInvoiceLineRepository _invoiceLineRepository;
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly IValidator<CreateARInvoiceDTO> _createARInvoiceDTOValidator;
        private readonly IValidator<UpdateARInvoiceDTO> _updateARInvoiceDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ARInvoiceService> _logger;

        public ARInvoiceService(IARInvoiceRepository invoiceRepository, IValidator<CreateARInvoiceDTO> createARInvoiceDTOValidator, IValidator<UpdateARInvoiceDTO> updateARInvoiceDTOValidator, ILogger<ARInvoiceService> logger, IARInvoiceLineRepository invoiceLineRepository, IMapper mapper, IBusinessPartnerService businessPartnerService)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _createARInvoiceDTOValidator = createARInvoiceDTOValidator;
            _updateARInvoiceDTOValidator = updateARInvoiceDTOValidator;
            _logger = logger;
            _businessPartnerService = businessPartnerService;
            _mapper = mapper;
        }

        public async Task<ARInvoiceDTO> CreateARInvoiceAndLinesAsync(Guid tenantId, Guid businessPartnerId, CreateARInvoiceDTO createInvoiceDTO, string userName)
        {
            // validation
            await _createARInvoiceDTOValidator.ValidateAndThrowAsync(createInvoiceDTO);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // insert invoice
                ARInvoice invoice = _mapper.Map<ARInvoice>(createInvoiceDTO);

                invoice.GrossAmount = createInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity);
                invoice.NetAmount = createInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity / (1 + invoiceLine.Tax));
                invoice.CreatedBy = userName;
                invoice.LastModificationBy = userName;
                invoice.BusinessPartnerId = businessPartnerId;

                invoice = await _invoiceRepository.InsertARInvoice(invoice);

                ARInvoiceDTO invoiceDTO = _mapper.Map<ARInvoiceDTO>(invoice);

                // inser invoice lines
                foreach (CreateARInvoiceLineDTO createInvoiceLineDTO in createInvoiceDTO.InvoiceLines)
                {
                    ARInvoiceLineDTO invoiceLineDTO = await CreateARInvoiceLineAsync(invoiceDTO.Id, createInvoiceLineDTO, userName);

                    invoiceDTO.InvoiceLines.Add(invoiceLineDTO);
                }

                transaction.Complete();

                return invoiceDTO;
            }
        }

        public async Task<IEnumerable<ARInvoiceDTO>> GetARInvoicesAsync(Guid tenantId, bool includeDeleted = false, int? page = null, int? pageSize = null)
        {
            List<ARInvoiceDTO> invoiceDTOs = new();

            IEnumerable<Invoice> invoices = await _invoiceRepository.GetARInvoicesAsync(tenantId, includeDeleted);

            Pagination.Paginate(ref invoices, page, pageSize);

            IEnumerable<ARInvoiceLineDTO> invoiceLines = await GetARInvoiceLinesAsync(tenantId, includeDeleted);
            IEnumerable<BusinessPartnerDTO> businessPartnerDTOs = await _businessPartnerService.GetBusinessPartnersAsync(tenantId, includeDeleted);
            Parallel.ForEach(invoices, (invoice) =>
            {
                ARInvoiceDTO invoiceDTO = _mapper.Map<ARInvoiceDTO>(invoice);
                invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();
                BusinessPartnerDTO businessPartnerDTO = businessPartnerDTOs.First(b => b.Id == invoice.BusinessPartnerId);
                invoiceDTO.BusinessPartner = _mapper.Map<BasicBusinessPartnerDTO>(businessPartnerDTO);
                invoiceDTOs.Add(invoiceDTO);
            });
            return invoiceDTOs;
        }

        public async Task<ARInvoiceDTO> GetARInvoiceByIdAsync(Guid tenantId, Guid invoiceId)
        {
            ARInvoiceDTO invoiceDTO = new();

            Invoice? invoice = await _invoiceRepository.GetARInvoiceByIdAsync(tenantId, invoiceId);

            if (invoice is null) throw new NotFoundException("AR Invoice not found");

            IEnumerable<ARInvoiceLineDTO> invoiceLines = await GetARInvoiceLinesAsync(tenantId);

            invoiceDTO = _mapper.Map<ARInvoiceDTO>(invoice);
            invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();

            return invoiceDTO;
        }

        public async Task<ARInvoiceDTO> UpdateARInvoiceAndLinesAsync(Guid tenantId, Guid invoiceId, UpdateARInvoiceDTO updateInvoiceDTO, string userName)
        {
            // validation
            await _updateARInvoiceDTOValidator.ValidateAndThrowAsync(updateInvoiceDTO);

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Get and Update Invoice
            ARInvoiceDTO actualInvoiceDTO = await GetARInvoiceByIdAsync(tenantId, invoiceId);

            ARInvoice invoice = _mapper.Map<ARInvoice>(actualInvoiceDTO);
            invoice.GrossAmount = updateInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity);
            invoice.NetAmount = updateInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity / (1 + invoiceLine.Tax));
            invoice.CreatedBy = userName;
            invoice.LastModificationBy = userName;

            invoice = await _invoiceRepository.UpdateARInvoiceAsync(invoice);

            List<Guid> updatedInvoiceLineIds = new();

            // Check Add/Update invoice lines
            foreach (UpdateARInvoiceLineDTO updateInvoiceLineDTO in updateInvoiceDTO.InvoiceLines)
            {
                if (updateInvoiceLineDTO.Id is not null)
                {
                    // Update invoice line
                    ARInvoiceLineDTO updatedInvoiceLine = await UpdateARInvoiceLineAsync((Guid)updateInvoiceLineDTO.Id, updateInvoiceLineDTO, userName);
                    updatedInvoiceLineIds.Add(updatedInvoiceLine.Id);
                }
                else
                {
                    // Add invoice line
                    CreateARInvoiceLineDTO createInvoiceLineDTO = _mapper.Map<CreateARInvoiceLineDTO>(updateInvoiceLineDTO);
                    await CreateARInvoiceLineAsync(actualInvoiceDTO.Id, createInvoiceLineDTO, userName);
                }
            }

            // Delete invoice lines not present in the update
            IEnumerable<Guid> invoiceLineIdsToDelete = actualInvoiceDTO.InvoiceLines
                .Where(invoiceLine => !updatedInvoiceLineIds.Contains(invoiceLine.Id))
                .Select(invoiceLine => invoiceLine.Id);

            foreach (Guid invoiceLineId in invoiceLineIdsToDelete)
            {
                await SetDeletedARInvoiceLineAsync(invoiceLineId, true, userName);
            }

            ARInvoiceDTO arInvoiceDTO = await GetARInvoiceByIdAsync(tenantId, invoiceId);

            transaction.Complete();

            return arInvoiceDTO;
        }

        public async Task SetDeletedARInvoiceAsync(Guid tenantId, Guid invoiceId, bool deleted, string userName)
        {
            // check if exists
            ARInvoiceDTO aRInvoiceDTO = await GetARInvoiceByIdAsync(tenantId, invoiceId);

            // check if already deleted/undeleted
            if (aRInvoiceDTO.Deleted == deleted)
            {
                string action = deleted ? "deleted" : "undeleted";
                throw new ConflictException($"Invoice already {action}");
            }

            await _invoiceRepository.SetDeletedARInvoiceAsync(invoiceId, deleted, userName);
        }

        private async Task<ARInvoiceLineDTO> CreateARInvoiceLineAsync(Guid invoiceId, CreateARInvoiceLineDTO createInvoiceLineDTO, string userName)
        {
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

        private async Task<ARInvoiceLineDTO> GetARInvoiceLineByIdAsync(Guid tenantId, Guid invoiceLineId)
        {
            ARInvoiceLine? invoiceLine = await _invoiceLineRepository.GetARInvoiceLineByIdAsync(tenantId, invoiceLineId);

            if (invoiceLine is null) throw new NotFoundException("AR Invoice line");

            return _mapper.Map<ARInvoiceLineDTO>(invoiceLine);
        }

        private async Task<ARInvoiceLineDTO> UpdateARInvoiceLineAsync(Guid invoiceLineId, UpdateARInvoiceLineDTO udpateInvoiceLineDTO, string userName, Guid? fixedAssetId = null)
        {

            ARInvoiceLine invoiceLine = _mapper.Map<ARInvoiceLine>(udpateInvoiceLineDTO);
            invoiceLine.Id = invoiceLineId;
            invoiceLine.LastModificationAt = DateTime.Now;
            invoiceLine.LastModificationBy = userName;

            invoiceLine = await _invoiceLineRepository.UpdateARInvoiceLineAsync(invoiceLine);

            return _mapper.Map<ARInvoiceLineDTO>(invoiceLine);
        }

        private async Task SetDeletedARInvoiceLineAsync(Guid invoiceLineId, bool deleted, string userName)
        {
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