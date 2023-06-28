using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;
using System.Transactions;

namespace AccountingAPI.Services
{
    public class APInvoiceService : IAPInvoiceService
    {
        private readonly IAPInvoiceRepository _invoiceRepository;
        private readonly IAPInvoiceLineService _invoiceLineService;
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly IValidator<CreateAPInvoiceDTO> _createAPInvoiceDTOValidator;
        private readonly IValidator<UpdateAPInvoiceDTO> _updateAPInvoiceDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<APInvoiceService> _logger;

        public APInvoiceService(IAPInvoiceRepository invoiceRepository
            , ILogger<APInvoiceService> logger
            , IAPInvoiceLineService invoiceLineService
            , IMapper mapper
            , IValidator<CreateAPInvoiceDTO> createAPInvoiceDTOValidator
            , IValidator<UpdateAPInvoiceDTO> updateAPInvoiceDTOValidator
            , IBusinessPartnerService businessPartnerService)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _invoiceLineService = invoiceLineService;
            _mapper = mapper;
            _createAPInvoiceDTOValidator = createAPInvoiceDTOValidator;
            _updateAPInvoiceDTOValidator = updateAPInvoiceDTOValidator;
            _businessPartnerService = businessPartnerService;
        }

        public async Task<APInvoiceDTO> CreateAPInvoiceAndLinesAsync(Guid tenantId, Guid businessPartnerId, CreateAPInvoiceDTO createInvoiceDTO, string userName)
        {
            // validation
            await _createAPInvoiceDTOValidator.ValidateAndThrowAsync(createInvoiceDTO);

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
                    BusinessPartnerDTO businessPartnerDTO = await _businessPartnerService.GetBusinessPartnerByIdAsync(tenantId, businessPartnerId);
                    invoiceDTO.BusinessPartner = _mapper.Map<BasicBusinessPartnerDTO>(businessPartnerDTO);
                    // insert invoice lines
                    Parallel.ForEach(createInvoiceDTO.InvoiceLines, async (createInvoiceLineDTO) =>
                    {
                        APInvoiceLineDTO invoiceLineDTO = await _invoiceLineService.CreateAPInvoiceLineAsync(tenantId, invoiceDTO.Id, createInvoiceLineDTO, invoice.Date, userName);

                        lock (invoiceDTO.InvoiceLines) // Lock access to the shared collection
                        {
                            invoiceDTO.InvoiceLines.Add(invoiceLineDTO);
                        }
                    });

                    transaction.Complete();

                    return invoiceDTO;
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<APInvoiceDTO>> GetAPInvoicesAsync(Guid tenantId, bool includeDeleted = false)
        {
            List<APInvoiceDTO> invoiceDTOs = new();
            IEnumerable<APInvoiceLineDTO> invoiceLinesDTOs = await _invoiceLineService.GetAPInvoiceLinesAsync(tenantId, includeDeleted);
            IEnumerable<BusinessPartnerDTO> businessPartnerDTOs = await _businessPartnerService.GetBusinessPartnersAsync(tenantId, includeDeleted);
            IEnumerable<Invoice> invoices = await _invoiceRepository.GetAPInvoicesAsync(tenantId, includeDeleted);
            Parallel.ForEach(invoices, (invoice) =>
            {
                APInvoiceDTO invoiceDTO = _mapper.Map<APInvoiceDTO>(invoice);
                invoiceDTO.InvoiceLines = invoiceLinesDTOs.Where(i => i.InvoiceId == invoice.Id).ToList();
                BusinessPartnerDTO businessPartnerDTO = businessPartnerDTOs.First(b => b.Id == invoice.BusinessPartnerId);
                invoiceDTO.BusinessPartner = _mapper.Map<BasicBusinessPartnerDTO>(businessPartnerDTO);
                lock (invoiceDTOs) // Lock access to the shared collection
                {
                    invoiceDTOs.Add(invoiceDTO);
                }
            });
            return invoiceDTOs;
        }

        public async Task<APInvoiceDTO> GetAPInvoiceByIdAsync(Guid tenantId, Guid invoiceId)
        {
            APInvoiceDTO invoiceDTO = new();
            IEnumerable<APInvoiceLineDTO> invoiceLines = await _invoiceLineService.GetAPInvoiceLinesAsync(tenantId);
            Invoice? invoice = await _invoiceRepository.GetAPInvoiceByIdAsync(tenantId, invoiceId);

            if (invoice is null) throw new NotFoundException("AP Invoice");

            invoiceDTO = _mapper.Map<APInvoiceDTO>(invoice);
            invoiceDTO.InvoiceLines = invoiceLines.Where(i => i.InvoiceId == invoice.Id).ToList();

            return invoiceDTO;
        }

        public async Task<APInvoiceDTO> UpdateAPInvoiceAndLinesAsync(Guid tenantId, Guid invoiceId, UpdateAPInvoiceDTO updateInvoiceDTO, string userName)
        {
            // validation
            await _updateAPInvoiceDTOValidator.ValidateAndThrowAsync(updateInvoiceDTO);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Get and Update Invoice
                    APInvoiceDTO actualInvoiceDTO = await GetAPInvoiceByIdAsync(tenantId, invoiceId);

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
                        if (updateInvoiceLineDTO.Id is not null)
                        {
                            // Update invoice line
                            APInvoiceLineDTO updatedInvoiceLine = await _invoiceLineService.UpdateAPInvoiceLineAsync(tenantId, (Guid)updateInvoiceLineDTO.Id, updateInvoiceLineDTO, userName, invoice.Date);
                            updateInvoiceLineIds.Add(updatedInvoiceLine.Id);
                        }
                        else
                        {
                            // Add invoice line
                            CreateAPInvoiceLineDTO createInvoiceLineDTO = _mapper.Map<CreateAPInvoiceLineDTO>(updateInvoiceLineDTO);
                            await _invoiceLineService.CreateAPInvoiceLineAsync(tenantId, actualInvoiceDTO.Id, createInvoiceLineDTO, invoice.Date, userName);
                        }
                    }

                    foreach (APInvoiceLineDTO actualInvoiceLineDTO in actualInvoiceDTO.InvoiceLines)
                    {
                        if (!updateInvoiceLineIds.Contains(actualInvoiceLineDTO.Id))
                        {
                            // delete invoice line
                            await _invoiceLineService.SetDeletedAPInvoiceLineAsync(tenantId, actualInvoiceLineDTO.Id, true, userName);
                        }
                    }

                    transaction.Complete();

                    return await GetAPInvoiceByIdAsync(tenantId, invoiceId);
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task SetDeletedAPInvoiceAsync(Guid tenantId, Guid invoiceId, bool deleted, string userName)
        {
            // check if exists
            await GetAPInvoiceByIdAsync(tenantId, invoiceId);

            await _invoiceRepository.SetDeletedAPInvoiceAsync(invoiceId, deleted, userName);
        }

    }
}