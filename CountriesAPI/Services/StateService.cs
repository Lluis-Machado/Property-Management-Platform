using AutoMapper;
using CountriesAPI.DTOs;
using CountriesAPI.Models;
using CountriesAPI.Repositories;

namespace CountriesAPI.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public StateService(IStateRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StateDTO>> GetStatesAsync(string? countryCode = null)
        {
            IEnumerable<State> states = await _stateRepository.GetStatesAsync(countryCode);

            return _mapper.Map<IEnumerable<State>, List<StateDTO>>(states);
        }
    }
}
