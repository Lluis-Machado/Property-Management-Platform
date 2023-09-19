using AutoMapper;
using Dapper;
using System.Text;
using TaxManagement.Context;
using TaxManagement.Models;

namespace TaxManagement.Repositories
{
    public class DeclarationRepository : IDeclarationRepository
    {
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public DeclarationRepository(DapperContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Declaration> InsertDeclarationAsync(Declaration declaration)
        {
            var parameters = new
            {
                declaration.DeclarantId,
                declaration.Status,
                declaration.CreatedByUser,
                declaration.LastUpdateByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Declarations (");
            queryBuilder.Append(" DeclarantId");
            queryBuilder.Append(" Status");
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
            queryBuilder.Append(" @Status");
            queryBuilder.Append(",@CreatedByUser");
            queryBuilder.Append(",@LastUpdateByUser");
            queryBuilder.Append(" )");

            var result = await _context
                .CreateConnection()
                .QuerySingleAsync<Declaration>(queryBuilder.ToString(), parameters);

            return result;

        }

        public async Task<IEnumerable<Declaration>> GetDeclarationsAsync(Guid declarantId, bool includeDeleted = false)
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
            queryBuilder.Append(" WHERE DeclarantId = @declarantId");
            if (includeDeleted == false) queryBuilder.Append(" AND Deleted = 0");


            var declarations = await _context
                .CreateConnection()
                .QueryAsync<Declaration>(queryBuilder.ToString(), parameters);

            return declarations;

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

            var declaration = await _context
                .CreateConnection()
                .QuerySingleAsync<Declaration>(queryBuilder.ToString(), parameters);

            return declaration;
        }

        public async Task<Declaration> UpdateDeclarationAsync(Declaration declaration)
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
            queryBuilder.Append(" OUTPUT INSERTED.* ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            Declaration result = await _context
                .CreateConnection()
                .QuerySingleAsync<Declaration>(queryBuilder.ToString(), parameters);

            return result;

        }

        public async Task<Declaration> SetDeletedDeclarationAsync(Guid declarantId, Guid declarationId, bool deleted, string? userName)
        {
            var parameters = new
            {
                declarationId,
                declarantId,
                deleted,
                userName,
                LastUpdateAt = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarations ");
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append(" ,LastUpdateByUser = @userName ");
            queryBuilder.Append(" ,LastUpdateAt = @LastUpdateAt ");
            queryBuilder.Append(" OUTPUT INSERTED.* ");
            queryBuilder.Append(" WHERE Id = @declarationId ");
            queryBuilder.Append(" And DeclarantId = @declarantId ");

            Declaration result = await _context
                .CreateConnection()
                .QuerySingleAsync<Declaration>(queryBuilder.ToString(), parameters);

            return result;
        }
    }
}
