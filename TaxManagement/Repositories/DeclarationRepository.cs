using Dapper;
using System.Text;
using TaxManagement.Context;
using TaxManagement.Models;

namespace TaxManagement.Repositories
{
    public class DeclarationRepository :IDeclarationRepository
    {
        private readonly DapperContext _context;
        public DeclarationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Declaration> InsertDeclarationAsync(Declaration declaration)
        {
            var parameters = new {
                declaration.DeclarantId,
                declaration.CreateUser,
                declaration.UpdateUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Declarations (");
            queryBuilder.Append(" DeclarantId");
            queryBuilder.Append(",CreateUser");
            queryBuilder.Append(",UpdateUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @DeclarantId");
            queryBuilder.Append(",@CreateUser");
            queryBuilder.Append(",@UpdateUser");
            queryBuilder.Append(" )");


            using var connection = _context.CreateConnection();

            Guid declarationId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            declaration.Id = declarationId;
            return declaration;
        }

        public async Task<IEnumerable<Declaration>> GetDeclarationsAsync(Guid? declarantId = null)
        {
            var parameters = new { declarantId };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ");
            queryBuilder.Append("Id");
            queryBuilder.Append(",DeclarantId");
            queryBuilder.Append(",Status");
            queryBuilder.Append(",CreateUser");
            queryBuilder.Append(",CreateDate");
            queryBuilder.Append(",UpdateUser");
            queryBuilder.Append(",UpdateDate");
            queryBuilder.Append(" FROM Declarations");
            if(declarantId != null) queryBuilder.Append(" WHERE DeclarantId = @declarantId");

            using var connection = _context.CreateConnection();
            var declarations = await connection.QueryAsync<Declaration>(queryBuilder.ToString(), parameters);
            return declarations.ToList();
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
            queryBuilder.Append(" ,CreateUser");
            queryBuilder.Append(" ,CreateDate");
            queryBuilder.Append(" ,UpdateUser");
            queryBuilder.Append(" ,UpdateDate");
            queryBuilder.Append(" FROM Declarations");
            queryBuilder.Append(" WHERE Id = @id");
            if(declarantId != null) queryBuilder.Append(" AND DeclarantId = @declarantId");

            using var connection = _context.CreateConnection();

            Declaration declaration = await connection.QuerySingleAsync<Declaration>(queryBuilder.ToString(), parameters);
            return declaration;
        }

        public async Task<int> UpdateDeclarationAsync(Declaration declaration)
        {
            var parameters = new
            {
                declaration.Id,
                declaration.Status,
                declaration.Deleted,
                declaration.UpdateUser,
                UpdateDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarations ");
            queryBuilder.Append("SET Status = @Status ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,UpdateUser = @UpdateUser ");
            queryBuilder.Append(" ,UpdateDate = @UpdateDate ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }

        public async Task<int> SetDeletedDeclarationAsync(Guid id, string user, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted,
                user,
                UpdateDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarations ");
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append(" ,UpdateUser = @user ");
            queryBuilder.Append(" ,UpdateDate = @UpdateDate ");
            queryBuilder.Append(" WHERE Id = @id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }


    }
}
