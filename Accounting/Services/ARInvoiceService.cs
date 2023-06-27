using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;
using System.Transactions;

namespace AccountingAPI.Services
{
    public class ARInvoiceService : IARInvoiceService
    {
        private readonly IARInvoiceRepository _invoiceRepository;
        private readonly IARInvoiceLineService _invoiceLineService;
        private readonly IValidator<CreateARInvoiceDTO> _createARInvoiceDTOValidator;
        private readonly IValidator<UpdateARInvoiceDTO> _updateARInvoiceDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ARInvoiceService> _logger;

        public ARInvoiceService(IARInvoiceRepository invoiceRepository, IValidator<CreateARInvoiceDTO> createARInvoiceDTOValidator, IValidator<UpdateARInvoiceDTO> updateARInvoiceDTOValidator, ILogger<ARInvoiceService> logger, IARInvoiceLineService invoiceLineService, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _createARInvoiceDTOValidator = createARInvoiceDTOValidator;
            _updateARInvoiceDTOValidator = updateARInvoiceDTOValidator;
            _logger = logger;
            _invoiceLineService = invoiceLineService;
            _mapper = mapper;
        }

        public async Task<ARInvoiceDTO> CreateARInvoiceAndLinesAsync(Guid tenantId, Guid businessPartnerId, CreateARInvoiceDTO createInvoiceDTO, string userName)
        {
            // validation
            await _createARInvoiceDTOValidator.ValidateAndThrowAsync(createInvoiceDTO);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
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
                        ARInvoiceLineDTO invoiceLineDTO = await _invoiceLineService.CreateARInvoiceLineAsync(tenantId, invoiceDTO.Id, createInvoiceLineDTO, userName);

                        invoiceDTO.InvoiceLines.Add(invoiceLineDTO);
                    }

                    transaction.Complete();

                    return invoiceDTO;
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<ARInvoiceDTO>> GetARInvoicesAsync(Guid tenantId, bool includeDeleted = false)
        {
            List<ARInvoiceDTO> invoiceDTOs = new();
            IEnumerable<ARInvoiceLineDTO> invoiceLines = await _invoiceLineService.GetARInvoiceLinesAsync(tenantId, includeDeleted);
            IEnumerable<Invoice> invoices = await _invoiceRepository.GetARInvoicesAsync(tenantId, includeDeleted);
            foreach (Invoice invoice in invoices)
            {
                ARInvoiceDTO invoiceDTO = _mapper.Map<ARInvoiceDTO>(invoice);
                invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();
                invoiceDTOs.Add(invoiceDTO);
            }
            return invoiceDTOs;
        }

        public async Task<ARInvoiceDTO> GetARInvoiceByIdAsync(Guid tenantId, Guid invoiceId)
        {
            ARInvoiceDTO invoiceDTO = new();

            Invoice? invoice = await _invoiceRepository.GetARInvoiceByIdAsync(tenantId, invoiceId);

            if (invoice is null) throw new NotFoundException("AR Invoice");

            IEnumerable<ARInvoiceLineDTO> invoiceLines = await _invoiceLineService.GetARInvoiceLinesAsync(tenantId);

            invoiceDTO = _mapper.Map<ARInvoiceDTO>(invoice);
            invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();

            return invoiceDTO;
        }

        public async Task<ARInvoiceDTO> UpdateARInvoiceAndLinesAsync(Guid tenantId, Guid invoiceId, UpdateARInvoiceDTO updateInvoiceDTO, string userName)
        {
            // validation
            await _updateARInvoiceDTOValidator.ValidateAndThrowAsync(updateInvoiceDTO);

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
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
                        ARInvoiceLineDTO updatedInvoiceLine = await _invoiceLineService.UpdateARInvoiceLineAsync(tenantId, (Guid)updateInvoiceLineDTO.Id, updateInvoiceLineDTO, userName);
                        updatedInvoiceLineIds.Add(updatedInvoiceLine.Id);
                    }
                    else
                    {
                        // Add invoice line
                        CreateARInvoiceLineDTO createInvoiceLineDTO = _mapper.Map<CreateARInvoiceLineDTO>(updateInvoiceLineDTO);
                        await _invoiceLineService.CreateARInvoiceLineAsync(tenantId, actualInvoiceDTO.Id, createInvoiceLineDTO, userName);
                    }
                }

                // Delete invoice lines not present in the update
                IEnumerable<Guid> invoiceLineIdsToDelete = actualInvoiceDTO.InvoiceLines
                    .Where(invoiceLine => !updatedInvoiceLineIds.Contains(invoiceLine.Id))
                    .Select(invoiceLine => invoiceLine.Id);

                foreach (Guid invoiceLineId in invoiceLineIdsToDelete)
                {
                    await _invoiceLineService.SetDeletedARInvoiceLineAsync(tenantId, invoiceLineId, true, userName);
                }

                transaction.Complete();

                return await GetARInvoiceByIdAsync(tenantId, invoiceId);
            }
            catch
            {
                throw;
            }
        }

        public async Task SetDeletedARInvoiceAsync(Guid tenantId, Guid invoiceId, bool deleted, string userName)
        {
            // check if exists
            await GetARInvoiceByIdAsync(tenantId, invoiceId);

            await _invoiceRepository.SetDeletedARInvoiceAsync(invoiceId, deleted, userName);
        }

    }
}