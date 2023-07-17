using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly IValidator<CreateLoanDTO> _createLoanDTOValidator;
        private readonly IValidator<UpdateLoanDTO> _updateLoanDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanService> _logger;

        public LoanService(ILoanRepository loanRepository, IBusinessPartnerService businessPartnerService, ILogger<LoanService> logger, IMapper mapper, IValidator<CreateLoanDTO> createLoanDTOValidator, IValidator<UpdateLoanDTO> updateLoanDTOValidator)
        {
            _loanRepository = loanRepository;
            _businessPartnerService = businessPartnerService;
            _createLoanDTOValidator = createLoanDTOValidator;
            _updateLoanDTOValidator = updateLoanDTOValidator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<LoanDTO> CreateLoanAsync(Guid tenantId, Guid businessPartnerId, CreateLoanDTO createLoanDTO, string userName)
        {
            // validation
            await _createLoanDTOValidator.ValidateAndThrowAsync(createLoanDTO);

            // check that business partner exists
            await _businessPartnerService.GetBusinessPartnerByIdAsync(tenantId, businessPartnerId);

            Loan loan = _mapper.Map<Loan>(createLoanDTO);
            loan.BusinessPartnerId = businessPartnerId;
            loan.CreatedBy = userName;
            loan.LastModificationBy = userName;

            loan = await _loanRepository.InsertLoanAsync(loan);

            return _mapper.Map<LoanDTO>(loan);
        }

        public async Task<IEnumerable<LoanDTO>> GetLoansAsync(Guid tenantId,int? page, int? pageSize, bool includeDeleted = false)
        {
            IEnumerable<Loan> loans = await _loanRepository.GetLoansAsync(tenantId, includeDeleted);
            
            if (page.HasValue && pageSize.HasValue)
            {
                loans = loans.Skip(page.Value - 1).Take(pageSize.Value);
            }
            return _mapper.Map<IEnumerable<LoanDTO>>(loans);
        }

        public async Task<LoanDTO> GetLoanByIdAsync(Guid tenantId, Guid LoanId)
        {
            Loan? loan = await _loanRepository.GetLoanByIdAsync(tenantId, LoanId);

            if (loan is null) throw new NotFoundException("Loan");

            return _mapper.Map<LoanDTO>(loan);
        }

        public async Task<LoanDTO> UpdateLoanAsync(Guid tenantId, Guid loanId, UpdateLoanDTO updateLoanDTO, string userName)
        {
            // validation
            await _updateLoanDTOValidator.ValidateAndThrowAsync(updateLoanDTO);

            // check if exists
            await GetLoanByIdAsync(tenantId, loanId);

            Loan loan = _mapper.Map<Loan>(updateLoanDTO);
            loan.Id = loanId;
            loan.LastModificationAt = DateTime.Now;
            loan.LastModificationBy = userName;

            loan = await _loanRepository.UpdateLoanAsync(loan);

            return _mapper.Map<LoanDTO>(loan);
        }

        public async Task SetDeletedLoanAsync(Guid tenantId, Guid loanId, bool deleted, string userName)
        {
            // check if exists
            LoanDTO loanDTO = await GetLoanByIdAsync(tenantId, loanId);

            // check if already deleted/undeleted
            if (loanDTO.Deleted == deleted)
            {
                string action = deleted ? "deleted" : "undeleted";
                throw new ConflictException($"Loan already {action}");
            }

            await _loanRepository.SetDeletedLoanAsync(loanId, deleted);
        }
    }
}