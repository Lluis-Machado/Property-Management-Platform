using System.Text.Json;

namespace CoreAPI.Services
{
    public interface IBaseClientService
    {
        Task<JsonDocument?> ReadAsync(string endpoint);
        Task<JsonDocument?> CreateAsync<T>(string endpoint, T data);
        // Update with JSON body
        Task<JsonDocument?> UpdateAsync<T>(string endpoint, T data);
        // Update with query parameters
        Task<JsonDocument?> UpdateAsync(string endpoint, string? parameters = null);
        Task<bool> DeleteAsync(string endpoint);
        Task<bool> UndeleteAsync(string endpoint);
    }
}
