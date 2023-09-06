using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OwnershipAPI.DTOs;
using OwnershipAPI.Models;
using OwnershipAPI.Repositories;

namespace OwnershipAPI.Services;

public class OwnershipService : IOwnershipService
{
    private readonly IMapper _mapper;
    private readonly IOwnershipRepository _ownershipRepo;
    private readonly IValidator<OwnershipDto> _ownershipValidator;

    public OwnershipService(IOwnershipRepository ownershipRepo, IValidator<OwnershipDto> ownershipValidator,
        IMapper mapper)
    {
        _ownershipRepo = ownershipRepo;
        _ownershipValidator = ownershipValidator;
        _mapper = mapper;
    }

    public async Task<ActionResult<OwnershipDto>> CreateOwnershipAsync(OwnershipDto ownershipDTO, string lastUser)
    {
        await _ownershipValidator.ValidateAndThrowAsync(ownershipDTO);

        var ownership = _mapper.Map<OwnershipDto, Ownership>(ownershipDTO);

        ownership = await _ownershipRepo.InsertOneAsync(ownership);

        ownershipDTO = _mapper.Map<Ownership, OwnershipDto>(ownership);

        return new CreatedResult($"ownership/{ownershipDTO.Id}", ownershipDTO);
    }

    public async Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipAsync()
    {
        var results = await _ownershipRepo.GetAsync();

        return new OkObjectResult(results);
    }

    public async Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipsOfContactAsync(Guid id)
    {
        var results = await _ownershipRepo.GetWithContactIdAsync(id);

        var ownershipDtos = _mapper.Map<List<OwnershipDetailedDto>>(results);

        foreach (var ownership in ownershipDtos)
        {
                var clientC = new PropertyServiceClient();
                var property = await clientC.GetPropertyByIdAsync(ownership.PropertyId);
                ownership.PropertyName = property!.Name;
        }
        return new OkObjectResult(ownershipDtos);
    }

    public async Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipsOfCompanyAsync(Guid id)
    {
        var results = await _ownershipRepo.GetWithCompanyIdAsync(id);

        var ownershipDtos = _mapper.Map<List<OwnershipDetailedDto>>(results);

        foreach (var ownership in ownershipDtos)
        {
            var clientC = new PropertyServiceClient();
            var property = await clientC.GetPropertyByIdAsync(ownership.PropertyId);
            ownership.PropertyName = property!.Name;
        }
        return new OkObjectResult(ownershipDtos);
    }

    public async Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipsOfPropertyAsync(Guid id)
    {
        var results = await _ownershipRepo.GetWithPropertyIdAsync(id);

        var ownershipDtos = _mapper.Map<List<OwnershipDetailedDto>>(results);

        foreach (var ownership in ownershipDtos)
        {
            if (ownership.OwnerType.ToLower() == "contact")
            {
                var clientC = new ContactServiceClient();
                var contact = await clientC.GetContactByIdAsync(ownership.OwnerId);
                ownership.OwnerName = $"{contact!.FirstName} {contact.LastName}";
            }
            if (ownership.OwnerType.ToLower() == "company")
            {
                var clientC = new CompanyServiceClient();
                var company = await clientC.GetCompanyByIdAsync(ownership.OwnerId);
                ownership.OwnerName = company!.Name;
            }
        }

        return new OkObjectResult(ownershipDtos);
    }

    public async Task<OwnershipDto> GetOwnershipByIdAsync(Guid id)
    {
        // Assuming you have a repository or data access layer to fetch the contact by ID
        var ownership = await _ownershipRepo.GetOwnershipByIdAsync(id);

        // You can map the retrieved contact entity to a ContactDTO if needed
        var ownershipDTO = _mapper.Map<Ownership, OwnershipDto>(ownership);

        return ownershipDTO;
    }

    public async Task<ActionResult<OwnershipDto>> UpsertOwnershipAsync(OwnershipDto ownershipDTO, string lastUser)
    {
        await _ownershipValidator.ValidateAndThrowAsync(ownershipDTO);

        var ownership = _mapper.Map<OwnershipDto, Ownership>(ownershipDTO);

        ownership.LastUpdateByUser = lastUser;
        ownership.LastUpdateAt = DateTime.UtcNow;

        if (ownership.Id is null || ownership.Id == Guid.Empty)
        {
            ownership.CreatedByUser = lastUser;
            ownership.CreatedAt = DateTime.UtcNow;
            ownership = await _ownershipRepo.InsertOneAsync(ownership);
        }
        else
        {
            ownership = await _ownershipRepo.UpdateAsync(ownership);
        }

        ownershipDTO = _mapper.Map<Ownership, OwnershipDto>(ownership);
        return new OkObjectResult(ownershipDTO);
    }

    public async Task<IActionResult> DeleteOwnershipAsync(Guid ownershipId, string lastUser)
    {
        var updateResult = await _ownershipRepo.SetDeleteAsync(ownershipId, true);
        if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Ownership not found");

        return new NoContentResult();
    }

    public async Task<IActionResult> UndeleteOwnershipAsync(Guid ownershipId, string lastUser)
    {
        var updateResult = await _ownershipRepo.SetDeleteAsync(ownershipId, false);
        if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

        return new NoContentResult();
    }
}