using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IOwnershipServiceClient
    {
        Task<JsonDocument?> GetOwnershipByIdAsync(Guid id, string? type);
        Task<JsonDocument?> CreateOwnership(string requestBody);
        Task<JsonDocument?> CreateOwnerships(string requestBody);

        Task DeleteOwnerships(Guid propertyId);
    }
}
