﻿using CountriesAPI.Contexts;
using CountriesAPI.Models;
using Dapper;
using System.Text;

namespace CountriesAPI.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IDapperContext _context;

        public CountryRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",CountryCode");
            queryBuilder.Append(",Name");
            queryBuilder.Append(" FROM Countries");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<Country>(queryBuilder.ToString());
        }
    }
}
