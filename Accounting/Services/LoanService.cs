using AutoMapper;
using AccountingAPI.Repositories;
using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanService> _logger;

        public LoanService(ILoanRepository loanRepository, ILogger<LoanService> logger, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<LoanDTO> CreateLoanAsync(CreateLoanDTO createLoanDTO, string userName)
        {
            Loan loan = _mapper.Map<Loan>(createLoanDTO);
            loan.CreatedBy = userName;
            loan.LastModificationBy = userName;
            loan = await _loanRepository.InsertLoanAsync(loan);
            return _mapper.Map<LoanDTO>(loan);  
        }
        public async Task<IEnumerable<LoanDTO>> GetLoansAsync(bool includeDeleted = false)
        {
            IEnumerable<Loan> loans = await _loanRepository.GetLoansAsync(includeDeleted);
            return _mapper.Map<IEnumerable<LoanDTO>>(loans);
        }

        public async Task<LoanDTO?> GetLoanByIdAsync(Guid LoanId)
        {
            Loan loan = await _loanRepository.GetLoanByIdAsync(LoanId);
            return _mapper.Map<LoanDTO>(loan);
        }

        public async Task<bool> CheckIfLoanExistsAsync(Guid LoanId)
        {
            return await _loanRepository.GetLoanByIdAsync(LoanId) != null;
        }

        public async Task<LoanDTO> UpdateLoanAsync(CreateLoanDTO createLoanDTO, string userName)
        {
            Loan loan = _mapper.Map<Loan>(createLoanDTO);
            loan.LastModificationAt = DateTime.Now;
            loan.LastModificationBy = userName;
            loan = await _loanRepository.UpdateLoanAsync(loan);
           return _mapper.Map<LoanDTO>(loan);
        }

        public async Task<int> SetDeletedLoanAsync(Guid loanId, bool deleted)
        {
            return await _loanRepository.SetDeletedLoanAsync(loanId,deleted);
        }
    }
}