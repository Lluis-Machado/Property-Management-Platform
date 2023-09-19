using AuditsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuditsAPI.Services
{
    public interface IAuditService
    {
        Task<ActionResult<IEnumerable<Audit>>> GetAsync(bool includeDeleted = false);
        Task<List<Dictionary<string, Tuple<object, object>>>> GetByIdAsync(Guid id);
        Task<List<Dictionary<string, Tuple<object, object>>>> GetByPropertyIdAsync(Guid id);
        Task<List<Dictionary<string, Tuple<object, object>>>> GetByCompanyIdAsync(Guid id);
        Task<List<Dictionary<string, Tuple<object, object>>>> GetByContactIdAsync(Guid id);
        Task<IActionResult> DeleteAsync(Guid id, string lastUser);
        Task<IActionResult> UndeleteAsync(Guid id, string lastUser);
    }
}
