using OwnershipAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace OwnershipAPI.Services
{
    public interface IOwnershipService
    {
        Task<ActionResult<OwnershipDTO>> CreateOwnershipAsync(OwnershipDTO ownership);
        Task<ActionResult<IEnumerable<OwnershipDTO>>> GetOwnershipAsync();
        Task<ActionResult<IEnumerable<OwnershipDTO>>> GetOwnershipsOfContactAsync(Guid contactId);
        Task<ActionResult<IEnumerable<OwnershipDTO>>> GetOwnershipsOfPropertyAsync(Guid propertyId);
        Task<ActionResult<OwnershipDTO>> UpdateOwnershipAsync(OwnershipDTO ownership, Guid contactId);
        Task<IActionResult> DeleteOwnershipAsync(Guid ownership);
        Task<IActionResult> UndeleteOwnershipAsync(Guid ownership);
        Task<OwnershipDTO> GetOwnershipByIdAsync(Guid id);
    }
}
