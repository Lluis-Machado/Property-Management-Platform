using Dapper;
using System.Text;
using TaxManagement.Context;
using TaxManagement.Models;

namespace TaxManagement.Repositories
{
    public class DeclarationRepository : IDeclarationRepository
    {
        private readonly DapperContext _context;
        public DeclarationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Declaration> InsertDeclarationAsync(Declaration declaration)
        {
            var parameters = new
            {
                declaration.DeclarantId,
                declaration.CreatedByUser,
                declaration.LastUpdateByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Declarations (");
            queryBuilder.Append(" DeclarantId");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateByUser");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.DeclarantId");
            queryBuilder.Append(",INSERTED.Status");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedByUser");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.LastUpdateByUser");
            queryBuilder.Append(",INSERTED.LastUpdateAt");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @DeclarantId");
            queryBuilder.Append(",@CreatedByUser");
            queryBuilder.Append(",@LastUpdateByUser");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Declaration>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Declaration>> GetDeclarationsAsync(Guid? declarantId = null)
        {
            var parameters = new { declarantId };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ");
            queryBuilder.Append("Id");
            queryBuilder.Append(",DeclarantId");
            queryBuilder.Append(",Status");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",LastUpdateByUser");
            queryBuilder.Append(",LastUpdateAt");
            queryBuilder.Append(" FROM Declarations");
            if (declarantId != null) queryBuilder.Append(" WHERE DeclarantId = @declarantId");

            return await _context
                .CreateConnection()
                .QueryAsync<Declaration>(queryBuilder.ToString(), parameters);
        }

        public async Task<Declaration> GetDeclarationByIdAsync(Guid id, Guid? declarantId = null)
        {
            var parameters = new
            {
                id,
                declarantId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,DeclarantId");
            queryBuilder.Append(" ,Status");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreatedByUser");
            queryBuilder.Append(" ,CreatedAt");
            queryBuilder.Append(" ,LastUpdateByUser");
            queryBuilder.Append(" ,LastUpdateAt");
            queryBuilder.Append(" FROM Declarations");
            queryBuilder.Append(" WHERE Id = @id");
            if (declarantId != null) queryBuilder.Append(" AND DeclarantId = @declarantId");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Declaration>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateDeclarationAsync(Declaration declaration)
        {
            var parameters = new
            {
                declaration.Id,
                declaration.Status,
                declaration.Deleted,
                declaration.LastUpdateByUser,
                LastUpdateAt = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarations ");
            queryBuilder.Append("SET Status = @Status ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,LastUpdateByUser = @LastUpdateByUser ");
            queryBuilder.Append(" ,LastUpdateAt = @LastUpdateAt ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedDeclarationAsync(Guid id, bool deleted, string? userName)
        {
            var parameters = new
            {
                id,
                deleted,
                userName,
                LastUpdateAt = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarations ");
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append(" ,LastUpdateByUser = @userName ");
            queryBuilder.Append(" ,LastUpdateAt = @LastUpdateAt ");
            queryBuilder.Append(" WHERE Id = @id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
