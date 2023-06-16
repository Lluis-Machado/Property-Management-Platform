using AccountingAPI.Repositories;
using AccountingAPI.DTOs;
using System.Transactions;
using AutoMapper;
using AccountingAPI.Models;

namespace AccountingAPI.Services
{
    public class APInvoiceService : IAPInvoiceService
    {
        private readonly IAPInvoiceRepository _invoiceRepository;
        private readonly IAPInvoiceLineService _invoiceLineService;
        private readonly IMapper _mapper;
        private readonly ILogger<APInvoiceService> _logger;

        public APInvoiceService(IAPInvoiceRepository invoiceRepository, ILogger<APInvoiceService> logger, IAPInvoiceLineService invoiceLineService, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _invoiceLineService = invoiceLineService;
            _mapper = mapper;
        }

        public async Task<APInvoiceDTO> CreateAPInvoiceAndLinesAsync(CreateAPInvoiceDTO createInvoiceDTO, string userName, Guid businessPartnerId)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
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

                    // insert invoice lines
                    foreach (CreateAPInvoiceLineDTO createInvoiceLineDTO in createInvoiceDTO.InvoiceLines)
                    {
                        APInvoiceLineDTO invoiceLineDTO = await _invoiceLineService.CreateAPInvoiceLineAsync(createInvoiceLineDTO, invoiceDTO.Id,invoice.Date, userName);

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

        public async Task<APInvoiceDTO> GetAPInvoiceByIdAsync(Guid invoiceId)
        {
            APInvoiceDTO invoiceDTO = new();
            IEnumerable<APInvoiceLineDTO> invoiceLines = await _invoiceLineService.GetAPInvoiceLinesAsync();
            Invoice? invoice = await _invoiceRepository.GetAPInvoiceByIdAsync(invoiceId);

            if (invoice == null) return invoiceDTO;

            invoiceDTO = _mapper.Map<APInvoiceDTO>(invoice);
            invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();

            return invoiceDTO;
        }

        public async Task<bool> CheckIfAPInvoiceExistsAsync(Guid invoiceId)
        {
            return await _invoiceRepository.GetAPInvoiceByIdAsync(invoiceId) != null;
        }

        public async Task<IEnumerable<APInvoiceDTO>> GetAPInvoicesAsync(bool includeDeleted = false)
        {
            List<APInvoiceDTO> invoiceDTOs = new();
            IEnumerable<APInvoiceLineDTO> invoiceLines = await _invoiceLineService.GetAPInvoiceLinesAsync(includeDeleted);
            IEnumerable<Invoice> invoices = await _invoiceRepository.GetAPInvoicesAsync(includeDeleted);
            foreach(Invoice invoice in invoices)
            {
                APInvoiceDTO invoiceDTO = _mapper.Map<APInvoiceDTO>(invoice);
                invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();
                invoiceDTOs.Add(invoiceDTO);
            }
            return invoiceDTOs;
        }

        public async Task<APInvoiceDTO> UpdateAPInvoiceAndLinesAsync(UpdateAPInvoiceDTO updateInvoiceDTO, string userName, Guid invoiceId)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Get and Update Invoice
                    APInvoiceDTO actualInvoiceDTO = await GetAPInvoiceByIdAsync(invoiceId);

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
                        if (updateInvoiceLineDTO.Id != null)
                        {
                            // Update invoice line
                            APInvoiceLineDTO updatedInvoiceLine = await _invoiceLineService.UpdateAPInvoiceLineAsync(updateInvoiceLineDTO, userName,(Guid)updateInvoiceLineDTO.Id, invoice.Date);
                            updateInvoiceLineIds.Add(updatedInvoiceLine.Id);
                        }
                        else
                        {
                            // Add invoice line
                            CreateAPInvoiceLineDTO createInvoiceLineDTO = _mapper.Map<CreateAPInvoiceLineDTO>(updateInvoiceLineDTO);
                            await _invoiceLineService.CreateAPInvoiceLineAsync(createInvoiceLineDTO, actualInvoiceDTO.Id,invoice.Date, userName);
                        }
                    }

                    foreach(APInvoiceLineDTO actualInvoiceLineDTO in actualInvoiceDTO.InvoiceLines)
                    {
                        if (!updateInvoiceLineIds.Contains(actualInvoiceLineDTO.Id))
                        {
                            // delete invoice line
                            await _invoiceLineService.SetDeletedAPInvoiceLineAsync(actualInvoiceLineDTO.Id, true);
                        }
                    }

                    transaction.Complete();

                    return await GetAPInvoiceByIdAsync(invoiceId);
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<int> SetDeletedAPInvoiceAsync(Guid invoiceId, bool deleted)
        {
            return await _invoiceRepository.SetDeletedAPInvoiceAsync(invoiceId, deleted);
        }

    }
}