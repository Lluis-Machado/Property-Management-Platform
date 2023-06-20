using AccountingAPI.Repositories;
using AccountingAPI.DTOs;
using System.Transactions;
using AutoMapper;
using AccountingAPI.Models;

namespace AccountingAPI.Services
{
    public class ARInvoiceService : IARInvoiceService
    {
        private readonly IARInvoiceRepository _invoiceRepository;
        private readonly IARInvoiceLineService _invoiceLineService;
        private readonly IMapper _mapper;
        private readonly ILogger<ARInvoiceService> _logger;

        public ARInvoiceService(IARInvoiceRepository invoiceRepository, ILogger<ARInvoiceService> logger, IARInvoiceLineService invoiceLineService, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _invoiceLineService = invoiceLineService;
            _mapper = mapper;
        }

        public async Task<ARInvoiceDTO> CreateARInvoiceAndLinesAsync(CreateARInvoiceDTO createInvoiceDTO, string userName, Guid businessPartnerId)
        {
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
                        ARInvoiceLineDTO invoiceLineDTO = await _invoiceLineService.CreateARInvoiceLineAsync(createInvoiceLineDTO, invoiceDTO.Id, userName);

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

        public async Task<ARInvoiceDTO> GetARInvoiceByIdAsync(Guid invoiceId)
        {
            ARInvoiceDTO invoiceDTO = new();
            IEnumerable<ARInvoiceLineDTO> invoiceLines = await _invoiceLineService.GetARInvoiceLinesAsync();
            Invoice? invoice = await _invoiceRepository.GetARInvoiceByIdAsync(invoiceId);

            if (invoice == null) return invoiceDTO;

            invoiceDTO = _mapper.Map<ARInvoiceDTO>(invoice);
            invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();

            return invoiceDTO;
        }

        public async Task<bool> CheckIfARInvoiceExistsAsync(Guid invoiceId)
        {
            return await _invoiceRepository.GetARInvoiceByIdAsync(invoiceId) != null;
        }

        public async Task<IEnumerable<ARInvoiceDTO>> GetARInvoicesAsync(bool includeDeleted = false)
        {
            List<ARInvoiceDTO> invoiceDTOs = new();
            IEnumerable<ARInvoiceLineDTO> invoiceLines = await _invoiceLineService.GetARInvoiceLinesAsync(includeDeleted);
            IEnumerable<Invoice> invoices = await _invoiceRepository.GetARInvoicesAsync(includeDeleted);
            foreach(Invoice invoice in invoices)
            {
                ARInvoiceDTO invoiceDTO = _mapper.Map<ARInvoiceDTO>(invoice);
                invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();
                invoiceDTOs.Add(invoiceDTO);
            }
            return invoiceDTOs;
        }

        public async Task<ARInvoiceDTO> UpdateARInvoiceAndLinesAsync(UpdateARInvoiceDTO updateInvoiceDTO, string userName, Guid invoiceId)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // Get and Update Invoice
                ARInvoiceDTO actualInvoiceDTO = await GetARInvoiceByIdAsync(invoiceId);

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
                    if (updateInvoiceLineDTO.Id != null)
                    {
                        // Update invoice line
                        ARInvoiceLineDTO updatedInvoiceLine = await _invoiceLineService.UpdateARInvoiceLineAsync(updateInvoiceLineDTO, userName, (Guid)updateInvoiceLineDTO.Id);
                        updatedInvoiceLineIds.Add(updatedInvoiceLine.Id);
                    }
                    else
                    {
                        // Add invoice line
                        CreateARInvoiceLineDTO createInvoiceLineDTO = _mapper.Map<CreateARInvoiceLineDTO>(updateInvoiceLineDTO);
                        await _invoiceLineService.CreateARInvoiceLineAsync(createInvoiceLineDTO, actualInvoiceDTO.Id, userName);
                    }
                }

                // Delete invoice lines not present in the update
                IEnumerable<Guid> invoiceLineIdsToDelete = actualInvoiceDTO.InvoiceLines
                    .Where(invoiceLine => !updatedInvoiceLineIds.Contains(invoiceLine.Id))
                    .Select(invoiceLine => invoiceLine.Id);

                foreach (Guid invoiceLineId in invoiceLineIdsToDelete)
                {
                    await _invoiceLineService.SetDeletedARInvoiceLineAsync(invoiceLineId, true);
                }

                transaction.Complete();

                return await GetARInvoiceByIdAsync(invoiceId);
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> SetDeletedARInvoiceAsync(Guid invoiceId, bool deleted)
        {
            return await _invoiceRepository.SetDeletedARInvoiceAsync(invoiceId, deleted);
        }

    }
}