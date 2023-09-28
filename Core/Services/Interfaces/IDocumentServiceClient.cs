using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IDocumentsServiceClient
    {
        Task<JsonDocument?> CreateArchive(string requestBody, string? type, string? id);

        //[Obsolete("Folders are automatically created in the Documents service depending on the Archive type")]
        //Task<string?> CreateFolder(string requestBody, string archiveId);
    }
}
