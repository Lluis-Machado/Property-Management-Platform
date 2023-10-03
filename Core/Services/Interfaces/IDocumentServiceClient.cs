using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IDocumentsServiceClient
    {
        Task<JsonDocument?> CreateArchiveAsync(string requestBody, string? type, string? id);
        Task DeleteArchiveAsync(Guid archiveId);

        //[Obsolete("Folders are automatically created in the Documents service depending on the Archive type")]
        //Task<string?> CreateFolder(string requestBody, string archiveId);
    }
}
