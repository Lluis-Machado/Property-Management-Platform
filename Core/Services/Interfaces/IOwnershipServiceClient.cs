using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IOwnershipServiceClient
    {
        Task<JsonDocument?> GetOwnershipByIdAsync(Guid id, string? type);
        Task<JsonDocument?> CreateOwnershipAsync(string requestBody);
        Task<JsonDocument?> CreateOwnershipsAsync(string requestBody);

        Task DeleteOwnershipsAsync(Guid propertyId);
    }
}
