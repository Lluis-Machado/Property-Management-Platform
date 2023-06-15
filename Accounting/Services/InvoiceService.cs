using AccountingAPI.Repositories;
using AccountingAPI.DTOs;
using System.Transactions;
using AutoMapper;
using AccountingAPI.Models;

namespace AccountingAPI.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceLineService _invoiceLineService;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IInvoiceRepository invoiceRepository, ILogger<InvoiceService> logger, IInvoiceLineService invoiceLineService, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _invoiceLineService = invoiceLineService;
            _mapper = mapper;
        }

        public async Task<InvoiceDTO> CreateInvoiceAndLinesAsync(CreateInvoiceDTO createInvoiceDTO, string userName, Guid businessPartnerId)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // insert invoice
                    Invoice invoice = _mapper.Map<Invoice>(createInvoiceDTO);

                    invoice.GrossAmount = createInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity);
                    invoice.NetAmount = createInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity / (1 + invoiceLine.Tax));
                    invoice.CreatedBy = userName;
                    invoice.LastModificationBy = userName;
                    invoice.BusinessPartnerId = businessPartnerId;

                    invoice = await _invoiceRepository.InsertInvoice(invoice);

                    InvoiceDTO invoiceDTO = _mapper.Map<InvoiceDTO>(invoice);

                    // inser invoice lines
                    foreach (CreateInvoiceLineDTO createInvoiceLineDTO in createInvoiceDTO.InvoiceLines)
                    {
                        InvoiceLineDTO invoiceLineDTO = await _invoiceLineService.CreateInvoiceLineAsync(createInvoiceLineDTO, invoiceDTO.Id, userName);

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

        public async Task<InvoiceDTO> GetInvoiceByIdAsync(Guid invoiceId)
        {
            InvoiceDTO invoiceDTO = new();
            IEnumerable<InvoiceLineDTO> invoiceLines = await _invoiceLineService.GetInvoiceLinesAsync();
            Invoice? invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);

            if (invoice == null) return invoiceDTO;

            invoiceDTO = _mapper.Map<InvoiceDTO>(invoice);
            invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();

            return invoiceDTO;
        }

        public async Task<bool> CheckIfInvoiceExistsAsync(Guid invoiceId)
        {
            return await _invoiceRepository.GetInvoiceByIdAsync(invoiceId) != null;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetInvoicesAsync(bool includeDeleted = false)
        {
            List<InvoiceDTO> invoiceDTOs = new();
            IEnumerable<InvoiceLineDTO> invoiceLines = await _invoiceLineService.GetInvoiceLinesAsync(includeDeleted);
            IEnumerable<Invoice> invoices = await _invoiceRepository.GetInvoicesAsync(includeDeleted);
            foreach(Invoice invoice in invoices)
            {
                InvoiceDTO invoiceDTO = _mapper.Map<InvoiceDTO>(invoice);
                invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();
                invoiceDTOs.Add(invoiceDTO);
            }
            return invoiceDTOs;
        }

        public async Task<InvoiceDTO> UpdateInvoiceAndLinesAsync(UpdateInvoiceDTO updateInvoiceDTO, string userName, Guid invoiceId)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Get and Update Invoice
                    InvoiceDTO actualInvoiceDTO = await GetInvoiceByIdAsync(invoiceId);

                    Invoice invoice = _mapper.Map<Invoice>(actualInvoiceDTO);
                    invoice.GrossAmount = updateInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity);
                    invoice.NetAmount = updateInvoiceDTO.InvoiceLines.Sum(invoiceLine => invoiceLine.UnitPrice * invoiceLine.Quantity / (1 + invoiceLine.Tax));
                    invoice.CreatedBy = userName;
                    invoice.LastModificationBy = userName;

                    invoice = await _invoiceRepository.UpdateInvoiceAsync(invoice);

                    List<Guid> updateInvoiceLineIds = new();

                    // check Add/Update invoice lines 
                    foreach (UpdateInvoiceLineDTO updateInvoiceLineDTO in updateInvoiceDTO.InvoiceLines)
                    {
                        if (updateInvoiceLineDTO.Id != null)
                        {
                            // Update invoice line
                            InvoiceLineDTO updatedInvoiceLine = await _invoiceLineService.UpdateInvoiceLineAsync(updateInvoiceLineDTO, userName, (Guid)updateInvoiceLineDTO.Id);
                            updateInvoiceLineIds.Add(updatedInvoiceLine.Id);
                        }
                        else
                        {
                            // Add invoice line
                            CreateInvoiceLineDTO createInvoiceLineDTO = _mapper.Map<CreateInvoiceLineDTO>(updateInvoiceLineDTO);
                            await _invoiceLineService.CreateInvoiceLineAsync(createInvoiceLineDTO, actualInvoiceDTO.Id, userName);
                        }
                    }

                    foreach(InvoiceLineDTO actualInvoiceLineDTO in actualInvoiceDTO.InvoiceLines)
                    {
                        if (!updateInvoiceLineIds.Contains(actualInvoiceLineDTO.Id))
                        {
                            // delete invoice line
                            await _invoiceLineService.SetDeletedInvoiceLineAsync(actualInvoiceLineDTO.Id, true);
                        }
                    }

                    transaction.Complete();

                    return await GetInvoiceByIdAsync(invoiceId);
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<int> SetDeleteInvoiceAsync(Guid invoiceId, bool deleted)
        {
            return await _invoiceRepository.SetDeleteInvoiceAsync(invoiceId, deleted);
        }

    }
}