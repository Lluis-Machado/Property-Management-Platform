using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using TaxManagement.Models;
using TaxManagement.Repositories;
using TaxManagement.Validators;
using TaxManagementAPI.DTOs;

namespace TaxManagementAPI.Services
{
    public class DeclarationService : IDeclarationService
    {
        private readonly IDeclarationRepository _declarationRepo;
        private readonly IDeclarantService _declarantService;
        private readonly IValidator<DeclarationDTO> _declarationValidator;
        private readonly IValidator<CreateDeclarationDTO> _createDeclarationValidator;
        private readonly IMapper _mapper;
        public DeclarationService(IDeclarantService declarantService, IDeclarationRepository declarationRepo, IValidator<DeclarationDTO> declarationValidator, IValidator<CreateDeclarationDTO> createDeclarationValidator, IMapper mapper)
        {
            _declarantService = declarantService;
            _declarationRepo = declarationRepo;
            _declarationValidator = declarationValidator;
            _createDeclarationValidator = createDeclarationValidator;
            _mapper = mapper;
        }
        public async Task<DeclarationDTO> CreateDeclarationAsync(CreateDeclarationDTO createDeclarationDTO, Guid declarantId, string userName)
        {
            // declaration validation
            ValidationResult validationResult = await _createDeclarationValidator.ValidateAsync(createDeclarationDTO);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.ToString("~"));
            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) throw new ValidationException("Declarant not found");

            var declaration = _mapper.Map<CreateDeclarationDTO, Declaration>(createDeclarationDTO);

            declaration.CreatedByUser = userName;
            declaration.LastUpdateByUser = userName;

            declaration = await _declarationRepo.InsertDeclarationAsync(declaration);

            var declarationDTO = _mapper.Map<Declaration, DeclarationDTO>(declaration);

            return declarationDTO;
        }

        public async Task<DeclarationDTO> UpdateDeclarationAsync(UpdateDeclarationDTO updateDeclarationDTO, Guid declarantId, Guid declarationId)
        {

            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) throw new ValidationException("Declarant not found");

            // declaration validation
            if (!await DeclarationExists(declarationId)) throw new ValidationException("Declaration not found");

            var declaration = _mapper.Map<UpdateDeclarationDTO, Declaration>(updateDeclarationDTO);

            declaration = await _declarationRepo.UpdateDeclarationAsync(declaration);

            var declarationDTO = _mapper.Map<Declaration, DeclarationDTO>(declaration);

            return declarationDTO;
        }
        public async Task<DeclarationDTO> SetDeletedDeclarationAsync(Guid declarantId, Guid declarationId, bool delete, string userName)
        {

            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) throw new ValidationException("Declarant not found");

            // declaration validation
            if (!await DeclarationExists(declarationId)) throw new ValidationException("Declaration not found");

            var declaration = await _declarationRepo.SetDeletedDeclarationAsync(declarantId, declarationId, delete, userName);

            var declarationDTO = _mapper.Map<Declaration, DeclarationDTO>(declaration);

            return declarationDTO;
        }

        public async Task<IEnumerable<DeclarationDTO>> GetDeclarationsAsync(Guid declarantId, bool includeDeleted = false)
        {
            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) throw new ValidationException("Declarant not found");

            var result = await _declarationRepo.GetDeclarationsAsync(declarantId);

            IEnumerable<DeclarationDTO> declarantDTOs = _mapper.Map<IEnumerable<DeclarationDTO>>(result);

            return declarantDTOs;
        }


        public async Task<bool> DeclarationExists(Guid declarantId)
        {
            Declaration? result = await _declarationRepo.GetDeclarationByIdAsync(declarantId);

            return (result != null);
        }

        public async Task<DeclarationDTO> DeleteDeclarationAsync(Guid declarantId, Guid declarationId, string userName)
        {
            var result = await _declarationRepo.SetDeletedDeclarationAsync(declarantId, declarationId, true, userName);

            var declarationDTO = _mapper.Map<Declaration, DeclarationDTO>(result);

            return declarationDTO;
        }
    }
}

