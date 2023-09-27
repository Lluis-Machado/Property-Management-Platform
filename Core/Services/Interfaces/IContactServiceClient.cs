using System;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IContactServiceClient
    {
        Task<string?> GetContactByIdAsync(Guid id);
        Task<string?> UpdateContactArchive(string contactId, string archiveId);
    }
}
