﻿using Dapper;
using System.Text;
using TaxManagement.Context;
using TaxManagement.Models;

namespace TaxManagement.Repositories
{
    public class DeclarantRepository: IDeclarantRepository
    {
        private readonly DapperContext _context;
        public DeclarantRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Declarant> InsertDeclarantAsync(Declarant declarant)
        {
            var parameters = new
            {
                declarant.Name
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Declarants (");
            queryBuilder.Append(" Name");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.Name");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @Name");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Declarant>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Declarant>> GetDeclarantsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" FROM Declarants ");

            return await _context
                .CreateConnection()
                .QueryAsync<Declarant>(queryBuilder.ToString());
        }

        public async Task<Declarant?> GetDeclarantByIdAsync(Guid id)
        {
            var parameters = new
            {
                id
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" FROM Declarants");
            queryBuilder.Append(" WHERE Id = @id");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Declarant?>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateDeclarantAsync(Declarant declarant)
        {
            var parameters = new
            {
                declarant.Id,
                declarant.Name,
                declarant.Deleted
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarants ");
            queryBuilder.Append("SET Name = @Name ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" WHERE Id = @Id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteDeclarantAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarants ");
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append(" WHERE Id = @id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
