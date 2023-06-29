using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using OwnershipAPI.Models;
using OwnershipAPI.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace OwnershipAPI.Services
{
    public class OwnershipService : IOwnershipService
    {
        private readonly IOwnershipRepository _ownershipRepo;
        private readonly IValidator<OwnershipDTO> _ownershipValidator;
        private readonly IMapper _mapper;


        public OwnershipService(IOwnershipRepository ownershipRepo, IValidator<OwnershipDTO> ownershipValidator, IMapper mapper)
        {
            _ownershipRepo = ownershipRepo;
            _ownershipValidator = ownershipValidator;
            _mapper = mapper;
        }

        public async Task<ActionResult<OwnershipDTO>> CreateOwnershipAsync(OwnershipDTO ownershipDTO)
        {
            var ownership = _mapper.Map<OwnershipDTO, Ownership>(ownershipDTO);

            ownership = await _ownershipRepo.InsertOneAsync(ownership);

            ownershipDTO = _mapper.Map<Ownership, OwnershipDTO>(ownership);

            return new CreatedResult($"ownership/{ownershipDTO.Id}", ownershipDTO);
        }

        public async Task<ActionResult<IEnumerable<OwnershipDTO>>> GetOwnershipAsync()
        {
            return new OkObjectResult(await _ownershipRepo.GetAsync());
        }

        public async Task<ActionResult<IEnumerable<OwnershipDTO>>> GetOwnershipsOfContactAsync(Guid id)
        {
            return new OkObjectResult(await _ownershipRepo.GetWithContactIdAsync(id));
        }

        public async Task<ActionResult<IEnumerable<OwnershipDTO>>> GetOwnershipsOfPropertyAsync(Guid id)
        {
            return new OkObjectResult(await _ownershipRepo.GetWithPropertyIdAsync(id));
        }

        public async Task<OwnershipDTO> GetOwnershipByIdAsync(Guid id)
        {
            // Assuming you have a repository or data access layer to fetch the contact by ID
            var ownership = await _ownershipRepo.GetOwnershipByIdAsync(id);

            // You can map the retrieved contact entity to a ContactDTO if needed
            var ownershipDTO = _mapper.Map<Ownership, OwnershipDTO>(ownership);

            return ownershipDTO;
        }

        public async Task<ActionResult<OwnershipDTO>> UpdateOwnershipAsync(OwnershipDTO ownershipDTO, Guid ownershipId)
        {
            var ownership = _mapper.Map<OwnershipDTO, Ownership>(ownershipDTO);

            ownership = await _ownershipRepo.UpdateAsync(ownership);

            ownershipDTO = _mapper.Map<Ownership, OwnershipDTO>(ownership);
            return new OkObjectResult(ownershipDTO);
        }

        public async Task<IActionResult> DeleteOwnershipAsync(Guid ownershipId)
        {
            var updateResult = await _ownershipRepo.SetDeleteAsync(ownershipId, true);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteOwnershipAsync(Guid ownershipId)
        {
            var updateResult = await _ownershipRepo.SetDeleteAsync(ownershipId, false);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

            return new NoContentResult();
        }

        public async Task<ValidationResult> ValidateOwnershipAsync(OwnershipDTO ownership)
        {
            // contact validation
            ValidationResult validationResult = await _ownershipValidator.ValidateAsync(ownership);
            return validationResult;
        }
    }


}
