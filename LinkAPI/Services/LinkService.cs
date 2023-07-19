using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LinkAPI.Models;
using LinkAPI.Repositories;
using LinkAPI.Dtos;
using LinkAPI.Validators;

namespace LinkAPI.Services;

public class LinkService : ILinkService
{
    private readonly IMapper _mapper;
    private readonly ILinkRepository _linkRepo;
    private readonly IValidator<LinkDto> _linkValidator;


    public LinkService(ILinkRepository linkRepo, IValidator<LinkDto> linkValidator,
        IMapper mapper)
    {
        _linkRepo = linkRepo;
        _linkValidator = linkValidator;
        _mapper = mapper;
    }

    public async Task<ActionResult<LinkDto>> CreateLinkAsync(LinkDto ownershipDTO, string lastUser)
    {
        await _linkValidator.ValidateAndThrowAsync(ownershipDTO);

        var ownership = _mapper.Map<LinkDto, Link>(ownershipDTO);

        ownership = await _linkRepo.InsertOneAsync(ownership);

        ownershipDTO = _mapper.Map<Link, LinkDto>(ownership);

        return new CreatedResult($"ownership/{ownershipDTO.Id}", ownershipDTO);
    }

    public async Task<ActionResult<IEnumerable<LinkDto>>> GetLinksAsync()
    {
        return new OkObjectResult(await _linkRepo.GetAsync());
    }

    public async Task<ActionResult<IEnumerable<LinkDto>>> GetLinksOfObjectAAsync(Guid id)
    {
        return new OkObjectResult(await _linkRepo.GetWithObjectAIdAsync(id));
    }

    public async Task<ActionResult<IEnumerable<LinkDto>>> GetLinksOfObjectBAsync(Guid id)
    {
        return new OkObjectResult(await _linkRepo.GetWithObjectBIdAsync(id));
    }

    public async Task<LinkDto> GetLinkByIdAsync(Guid id)
    {
        // Assuming you have a repository or data access layer to fetch the contact by ID
        var link = await _linkRepo.GetLinkByIdAsync(id);

        // You can map the retrieved contact entity to a ContactDTO if needed
        var linkDto = _mapper.Map<Link, LinkDto>(link);

        return linkDto;
    }

    public async Task<ActionResult<LinkDto>> UpsertLinkAsync(LinkDto ownershipDTO, string lastUser)
    {
        await _linkValidator.ValidateAndThrowAsync(ownershipDTO);

        var ownership = _mapper.Map<LinkDto, Link>(ownershipDTO);

        ownership.LastUpdateByUser = lastUser;
        ownership.LastUpdateAt = DateTime.UtcNow;

        if (ownership.Id is null || ownership.Id == Guid.Empty)
        {
            ownership.CreatedByUser = lastUser;
            ownership.CreatedAt = DateTime.UtcNow;
            ownership = await _linkRepo.InsertOneAsync(ownership);
        }
        else
        {
            ownership = await _linkRepo.UpdateAsync(ownership);
        }

        ownershipDTO = _mapper.Map<Link, LinkDto>(ownership);
        return new OkObjectResult(ownershipDTO);
    }

    public async Task<IActionResult> DeleteLinkAsync(Guid ownershipId, string lastUser)
    {
        var updateResult = await _linkRepo.SetDeleteAsync(ownershipId, true);
        if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

        return new NoContentResult();
    }

    public async Task<IActionResult> UndeleteLinkAsync(Guid ownershipId, string lastUser)
    {
        var updateResult = await _linkRepo.SetDeleteAsync(ownershipId, false);
        if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

        return new NoContentResult();
    }
}