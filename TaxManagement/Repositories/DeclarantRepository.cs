using Dapper;
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

            using var connection = _context.CreateConnection();

            declarant = await connection.QuerySingleAsync<Declarant>(queryBuilder.ToString(), parameters);
            return declarant;
        }

        public async Task<IEnumerable<Declarant>> GetDeclarantsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" FROM Declarants ");

            using var connection = _context.CreateConnection();

            IEnumerable<Declarant> declarants = await connection.QueryAsync<Declarant>(queryBuilder.ToString());
            return declarants;
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

            using var connection = _context.CreateConnection();

            Declarant? declarant = await connection.QuerySingleAsync<Declarant?>(queryBuilder.ToString(), parameters);
            return declarant;
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

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
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

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }
    }
}
