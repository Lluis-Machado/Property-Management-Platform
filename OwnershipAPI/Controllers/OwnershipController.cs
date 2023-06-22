using OwnershipAPI.Models;
using OwnershipAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using AutoMapper;

namespace OwnershipAPI.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("ownership")]
    public class OwnershipController : ControllerBase
    {
        private readonly IOwnershipService _ownershipService;
        private readonly IValidator<OwnershipDTO> _ownershipValidator;

        public OwnershipController(IOwnershipService ownershipService, IValidator<OwnershipDTO> ownershipValidator)
        {
            _ownershipService = ownershipService;
            _ownershipValidator = ownershipValidator;           
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<OwnershipDTO>> CreateAsync([FromBody] OwnershipDTO ownershipDTO)
        {
            if (ownershipDTO == null) return new BadRequestObjectResult("Incorrect body format");
            if (ownershipDTO.Id != Guid.Empty) return new BadRequestObjectResult("Id field must be empty");

            // Ownership validation
            ValidationResult validationResult = await _ownershipValidator.ValidateAsync(ownershipDTO);
            if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.ToString("~"));

            return await _ownershipService.CreateOwnershipAsync(ownershipDTO);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<OwnershipDTO>>> GetAsync()
        {
            return await _ownershipService.GetOwnershipAsync();
        }

        [HttpGet("{id}/contact")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<OwnershipDTO>>> GetOwnershipsOfContactAsync(Guid id)
        {
            return await _ownershipService.GetOwnershipsOfContactAsync(id);
        }

        [HttpGet("{id}/property")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<OwnershipDTO>>> GetOwnershipsOfPropertyAsync(Guid id)
        {
            return await _ownershipService.GetOwnershipsOfPropertyAsync(id);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<OwnershipDTO>> GetByIdAsync(Guid id)
        {
            var ownership = await _ownershipService.GetOwnershipByIdAsync(id);
            if (ownership == null)
            {
                return NotFound();
            }

            return ownership;
        }




        [HttpPatch]
        [Route("{ownershipId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<OwnershipDTO>> UpdateAsync([FromBody] OwnershipDTO ownershipDTO, Guid ownershipId)
        {
            // validations
            if (ownershipDTO == null) return new BadRequestObjectResult("Incorrect body format");
            if (ownershipDTO.Id != ownershipId) return new BadRequestObjectResult("Ownership Id from body is incorrect");

            // contact validation
            ValidationResult validationResult = await _ownershipValidator.ValidateAsync(ownershipDTO);
            if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.ToString("~"));

            return await _ownershipService.UpdateOwnershipAsync(ownershipDTO, ownershipId);
        }

        [HttpDelete]
        [Route("{ownershipId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid ownershipId)
        {
            return await _ownershipService.DeleteOwnershipAsync(ownershipId);
        }

        [HttpPatch]
        [Route("{ownershipId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid ownershipId)
        {
            return await _ownershipService.UndeleteOwnershipAsync(ownershipId);
        }
    }
}
