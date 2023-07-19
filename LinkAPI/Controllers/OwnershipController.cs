using System.Net;
using Microsoft.AspNetCore.Mvc;
using LinkAPI.Exceptions;
using LinkAPI.Models;
using LinkAPI.Services;
using LinkAPI.Validators;
using LinkAPI.Dtos;

namespace LinkAPI.Controllers;

// [Authorize]
[ApiController]
[Route("link")]
public class LinkController : ControllerBase
{
    private readonly ILinkService _ownershipService;

    public LinkController(ILinkService ownershipService)
    {
        _ownershipService = ownershipService;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<LinkDto>> CreateAsync([FromBody] LinkDto ownershipDto)
    {
        // validations
        if (ownershipDto is null) return new BadRequestObjectResult("Incorrect body format");

        var lastUser = "test";
        lastUser = UserNameValidator.GetValidatedUserName(lastUser);

        return await _ownershipService.UpsertLinkAsync(ownershipDto, lastUser);
    }
    
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<LinkDto>>> GetAsync()
    {
        return await _ownershipService.GetLinksAsync();
    }

    [HttpGet("{linkId}/objecta")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<LinkDto>>> GetLinksOfObjectBAsync(Guid linkId)
    {
        return await _ownershipService.GetLinksOfObjectBAsync(linkId);
    }

    [HttpGet("{id}/objectb")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<LinkDto>>> GetLinksOfObjectAAsync(Guid id)
    {
        return await _ownershipService.GetLinksOfObjectAAsync(id);
    }

    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<LinkDto>> GetByIdAsync(Guid id)
    {
        var ownership = await _ownershipService.GetLinkByIdAsync(id);
        return ownership ?? throw new NotFoundException("Link");
    }

    [HttpPatch]
    //[Route("{ownershipId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<LinkDto>> UpsertAsync([FromBody] LinkDto ownershipDTO)
    {
        // validations
        if (ownershipDTO is null) return new BadRequestObjectResult("Incorrect body format");

        var lastUser = "test";
        lastUser = UserNameValidator.GetValidatedUserName(lastUser);

        return await _ownershipService.UpsertLinkAsync(ownershipDTO, lastUser);
    }

    [HttpDelete]
    [Route("{linkId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid linkId)
    {
        var lastUser = "test";
        lastUser = UserNameValidator.GetValidatedUserName(lastUser);

        return await _ownershipService.DeleteLinkAsync(linkId, lastUser);
    }

    [HttpPatch]
    [Route("{linkId}/undelete")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UndeleteAsync(Guid linkId)
    {
        var lastUser = "test";
        lastUser = UserNameValidator.GetValidatedUserName(lastUser);

        return await _ownershipService.UndeleteLinkAsync(linkId, lastUser);
    }
}