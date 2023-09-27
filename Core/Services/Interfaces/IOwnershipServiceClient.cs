using System;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IOwnershipServiceClient
    {
        Task<string?> GetOwnershipByIdAsync(Guid id);
        Task<string?> CreateOwnership(string requestBody);
        Task<string?> CreateOwnerships(string requestBody);
    }
}
