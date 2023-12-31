﻿using CountriesAPI.Models;

namespace CountriesAPI.Repositories
{
    public interface IStateRepository
    {
        Task<IEnumerable<State>> GetStatesAsync(int? countryId);
    }
}
