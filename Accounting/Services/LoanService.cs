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
        private readonly IValidator<CreateLoanDTO> _createLoanDTOValidator;
        private readonly IValidator<UpdateLoanDTO> _updateLoanDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanService> _logger;

        public LoanService(ILoanRepository loanRepository, ILogger<LoanService> logger, IMapper mapper, IValidator<CreateLoanDTO> createLoanDTOValidator, IValidator<UpdateLoanDTO> updateLoanDTOValidator)
        {
            _loanRepository = loanRepository;
            _createLoanDTOValidator = createLoanDTOValidator;
            _updateLoanDTOValidator = updateLoanDTOValidator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<LoanDTO> CreateLoanAsync(Guid tenantId, Guid BusinessPartnerId, CreateLoanDTO createLoanDTO, string userName)
        {
            // validation
            await _createLoanDTOValidator.ValidateAndThrowAsync(createLoanDTO);

            Loan loan = _mapper.Map<Loan>(createLoanDTO);
            loan.CreatedBy = userName;
            loan.LastModificationBy = userName;

            loan = await _loanRepository.InsertLoanAsync(loan);

            return _mapper.Map<LoanDTO>(loan);
        }

        public async Task<IEnumerable<LoanDTO>> GetLoansAsync(Guid tenantId, bool includeDeleted = false)
        {
            IEnumerable<Loan> loans = await _loanRepository.GetLoansAsync(tenantId, includeDeleted);
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
            await GetLoanByIdAsync(tenantId, loanId);

            await _loanRepository.SetDeletedLoanAsync(loanId, deleted);
        }
    }
}