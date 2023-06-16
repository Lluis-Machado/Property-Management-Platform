using Documents.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archives.Services
{
    public interface IArchivesService
    {
        Task<Archive> CreateArchiveAsync(Archive archive);
        Task<IEnumerable<Archive>> GetArchivesAsync(bool includeDeleted = false);
        Task DeleteArchiveAsync(Guid archiveId);
        Task UndeleteArchiveAsync(Guid archiveId);
    }
}
