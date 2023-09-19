using LinkAPI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LinkAPI.Services
{
    public interface ILinkService
    {
        Task<ActionResult<LinkDto>> CreateLinkAsync(LinkDto link, string lastUser);
        Task<ActionResult<IEnumerable<LinkDto>>> GetLinksAsync();
        Task<ActionResult<IEnumerable<LinkDto>>> GetLinksOfObjectAAsync(Guid objectAId);
        Task<ActionResult<IEnumerable<LinkDto>>> GetLinksOfObjectBAsync(Guid objectBId);
        Task<ActionResult<LinkDto>> UpsertLinkAsync(LinkDto link, string lastUser);
        Task<IActionResult> DeleteLinkAsync(Guid linkId, string lastUser);
        Task<IActionResult> UndeleteLinkAsync(Guid linkId, string lastUser);
        Task<LinkDto> GetLinkByIdAsync(Guid linkId);
    }
}
