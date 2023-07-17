using OwnershipAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using OwnershipAPI.DTOs;

namespace OwnershipAPI.Services
{
    public interface IOwnershipService
    {
        Task<ActionResult<OwnershipDto>> CreateOwnershipAsync(OwnershipDto ownership, string lastUser);
        Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipAsync();
        Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipsOfContactAsync(Guid contactId);
        Task<ActionResult<IEnumerable<OwnershipDto>>> GetOwnershipsOfPropertyAsync(Guid propertyId);
        Task<ActionResult<OwnershipDto>> UpsertOwnershipAsync(OwnershipDto ownership, string lastUser);
        Task<IActionResult> DeleteOwnershipAsync(Guid ownership, string lastUser);
        Task<IActionResult> UndeleteOwnershipAsync(Guid ownership, string lastUser);
        Task<OwnershipDto> GetOwnershipByIdAsync(Guid id);
    }
}
