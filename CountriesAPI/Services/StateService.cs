using AutoMapper;
using CountriesAPI.DTOs;
using CountriesAPI.Models;
using CountriesAPI.Repositories;

namespace CountriesAPI.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly IStateTranslationRepository _stateTranslationRepository;
        private readonly IMapper _mapper;

        public StateService(IStateRepository stateRepository, IStateTranslationRepository stateTranslationRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _stateTranslationRepository = stateTranslationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StateDTO>> GetStatesAsync(string? languageCode = null)
        {
            if (languageCode is not null) return await GetTranslatedStatesAsync(languageCode);

            return await GetAllStatesAsync();
        }

        public async Task<IEnumerable<StateDTO>> GetStatesByCountryAsync(int countryId, string? languageCode = null)
        {
            if (languageCode is not null) return await GetTranslatedStatesAsync(languageCode, countryId);

            return await GetAllStatesAsync(countryId);
        }

        private async Task<IEnumerable<StateDTO>> GetTranslatedStatesAsync(string languageCode, int? countryId = null)
        {
            List<StateDTO> stateDTOs = new();

            IEnumerable<State> states = await _stateRepository.GetStatesAsync(countryId);

            IEnumerable<StateTranslation> stateTranslations = await _stateTranslationRepository.GetStateTranslationsAsync();

            foreach (State state in states)
            {
                StateDTO stateDTO = _mapper.Map<StateDTO>(state);

                if (stateTranslations.Any())
                {
                    StateTranslation? stateTranslation = stateTranslations.FirstOrDefault(c => c.StateId == state.Id && c.LanguageCode == languageCode);

                    if (stateTranslation is not null)
                    {
                        stateDTO.LanguageCode = languageCode;
                        stateDTO.Name = stateTranslation.Translation;
                    }
                }
                stateDTOs.Add(stateDTO);
            }
            return stateDTOs;
        }

        private async Task<IEnumerable<StateDTO>> GetAllStatesAsync(int? countryId = null)
        {
            IEnumerable<State> states = await _stateRepository.GetStatesAsync(countryId);
            return _mapper.Map<IEnumerable<State>, List<StateDTO>>(states);
        }
    }
}
