using CountriesAPI.DTOs;

namespace CountriesAPI.Services
{
    public interface IStateService
    {
        Task<IEnumerable<StateDTO>> GetStatesAsync(string? countryCode = null);
    }
}
