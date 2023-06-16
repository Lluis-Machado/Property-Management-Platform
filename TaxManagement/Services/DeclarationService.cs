using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaxManagement.Models;
using TaxManagement.Repositories;
using TaxManagement.Validators;
using TaxManagementAPI.DTOs;
using FluentValidation.Results;
using System.Security.Claims;
using System.Xml.Linq;
using AutoMapper;

namespace TaxManagementAPI.Services
{
    public class DeclarationService : IDeclarationService
    {
        private readonly IDeclarationRepository _declarationRepo;
        private readonly IDeclarantRepository _declarantRepo;
        private readonly IValidator<DeclarationDTO> _declarationValidator;
        private readonly IMapper _mapper;
        public DeclarationService(IDeclarationRepository declarantRepo, IDeclarantRepository declarationRepo, IValidator<DeclarationDTO> declarationValidator, IMapper mapper)
        {
            _declarationRepo = declarantRepo;
            _declarantRepo = declarationRepo;
            _declarationValidator = declarationValidator;
            _mapper = mapper;            
        }
        public async Task<DeclarationDTO> CreateDeclarationAsync(CreateDeclarationDTO createDeclarationDTO, string userName)
        {
            var declaration = _mapper.Map<CreateDeclarationDTO, Declaration>(createDeclarationDTO);

            declaration.CreatedByUser = userName;
            declaration.LastUpdateByUser = userName;

            declaration = await _declarationRepo.InsertDeclarationAsync(declaration);

            var declarationDTO = _mapper.Map<Declaration, DeclarationDTO>(declaration);


            return declarationDTO;
        }

        public async Task<DeclarationDTO> UpdateDeclarationAsync(UpdateDeclarationDTO updateDeclarationDTO)
        {
            var declaration = _mapper.Map<UpdateDeclarationDTO, Declaration>(updateDeclarationDTO);
                   
            declaration = await _declarationRepo.UpdateDeclarationAsync(declaration);

            var declarationDTO = _mapper.Map<Declaration, DeclarationDTO>(declaration);

            return declarationDTO;
        }
        public async Task<DeclarationDTO> SetDeletedDeclarationAsync(Guid declarantId, Guid declarationId, bool delete, string userName)
        {
            var declaration = await _declarationRepo.SetDeletedDeclarationAsync(declarantId, declarationId, delete, userName);

            var declarationDTO = _mapper.Map<Declaration, DeclarationDTO>(declaration);

            return declarationDTO;
        }

        public async Task<IEnumerable<DeclarationDTO>> GetDeclarationsAsync(Guid declarantId)
        {
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

