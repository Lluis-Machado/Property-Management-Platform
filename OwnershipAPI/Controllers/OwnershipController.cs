using System.Net;
using Microsoft.AspNetCore.Mvc;
using OwnershipAPI.DTOs;
using OwnershipAPI.Exceptions;
using OwnershipAPI.Models;
using OwnershipAPI.Services;
using OwnershipAPI.Validators;

namespace OwnershipAPI.Controllers;

// [Authorize]
[ApiController]
[Route("ownership")]
public class OwnershipController : ControllerBase
{
    private readonly IOwnershipService _ownershipService;

    public OwnershipController(IOwnershipService ownershipService)
    {
        _ownershipService = ownershipService;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<OwnershipDto>> CreateAsync([FromBody] OwnershipDto ownershipDto)
    {
        // validations
        if (ownershipDto is null) return new BadRequestObjectResult("Incorrect body format");

        var lastUser = "test";
        lastUser = UserNameValidator.GetValidatedUserName(lastUser);

        return await _ownershipService.UpsertOwnershipAsync(ownershipDto, lastUser);
    }
    
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<OwnershipDto>>> GetAsync()
    {
        return await _ownershipService.GetOwnershipAsync();
    }

    [HttpGet("{id}/contact")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipsOfContactAsync(Guid id)
    {
        return await _ownershipService.GetOwnershipsOfContactAsync(id);
    }

    [HttpGet("{id}/property")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipsOfPropertyAsync(Guid id)
    {
        return await _ownershipService.GetOwnershipsOfPropertyAsync(id);
    }

    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<OwnershipDto>> GetByIdAsync(Guid id)
    {
        var ownership = await _ownershipService.GetOwnershipByIdAsync(id);
        return ownership ?? throw new NotFoundException("Ownership");
    }

    [HttpPatch]
    //[Route("{ownershipId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<OwnershipDto>> UpsertAsync([FromBody] OwnershipDto ownershipDTO)
    {
        // validations
        if (ownershipDTO is null) return new BadRequestObjectResult("Incorrect body format");

        var lastUser = "test";
        lastUser = UserNameValidator.GetValidatedUserName(lastUser);

        return await _ownershipService.UpsertOwnershipAsync(ownershipDTO, lastUser);
    }

    [HttpDelete]
    [Route("{ownershipId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid ownershipId)
    {
        var lastUser = "test";
        lastUser = UserNameValidator.GetValidatedUserName(lastUser);

        return await _ownershipService.DeleteOwnershipAsync(ownershipId, lastUser);
    }

    [HttpPatch]
    [Route("{ownershipId}/undelete")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UndeleteAsync(Guid ownershipId)
    {
        var lastUser = "test";
        lastUser = UserNameValidator.GetValidatedUserName(lastUser);

        return await _ownershipService.UndeleteOwnershipAsync(ownershipId, lastUser);
    }
}