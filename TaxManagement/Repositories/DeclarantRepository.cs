using AutoMapper;
using Dapper;
using System.Text;
using TaxManagement.Context;
using TaxManagement.Models;

namespace TaxManagement.Repositories
{
    public class DeclarantRepository : IDeclarantRepository
    {
        private readonly DapperContext _context;
        private readonly IMapper mapper;
        public DeclarantRepository(DapperContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        public async Task<Declarant> InsertDeclarantAsync(Declarant declarant)
        {
            var parameters = new
            {
                declarant.Name,
                declarant.CreatedByUser,
                declarant.LastUpdateByUser
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Declarants (");
            queryBuilder.Append(" Name");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateByUser");
            queryBuilder.Append(")OUTPUT INSERTED.*");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(",@Name");
            queryBuilder.Append(",@CreatedByUser");
            queryBuilder.Append(",@LastUpdateByUser");
            queryBuilder.Append(" )");

            var result = await _context
                         .CreateConnection()
                         .QuerySingleAsync<Declarant>(queryBuilder.ToString(), parameters);

            return result;

        }

        public async Task<IEnumerable<Declarant>> GetDeclarantsAsync(bool includeDeleted = false)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateAt");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateByUser");
            queryBuilder.Append(" FROM Declarants ");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");

            var result = await _context
                        .CreateConnection()
                        .QueryAsync<Declarant>(queryBuilder.ToString());

            return result;
        }

        public async Task<Declarant?> GetDeclarantByIdAsync(Guid id)
        {
            var parameters = new
            {
                id
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateAt");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateByUser");
            queryBuilder.Append(" FROM Declarants");
            queryBuilder.Append(" WHERE Id = @id");

            var result = await _context
                .CreateConnection()
                .QuerySingleAsync<Declarant?>(queryBuilder.ToString(), parameters);

            return result;
        }

        public async Task<Declarant> UpdateDeclarantAsync(Declarant declarant)
        {
            var oldDeclarant = _context.CreateConnection().QuerySingleOrDefault<Declarant>("SELECT * FROM Declarants WHERE Id = @Id", new { Id = declarant.Id });

            if (oldDeclarant == null)
            {
                throw new Exception($"Declarant with ID {declarant.Id} not found in database");
            }

            var parameters = new
            {
                declarant.Id,
                declarant.Name,
                declarant.Deleted,
                declarant.LastUpdateByUser,
                declarant.LastUpdateAt
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarants ");
            queryBuilder.Append("SET Name = @Name ");
            queryBuilder.Append(",Deleted = @Deleted ");
            queryBuilder.Append(",LastUpdateByUser = @LastUpdateByUser ");
            queryBuilder.Append(",LastUpdateAt = @LastUpdateAt ");
            queryBuilder.Append(" WHERE Id = @Id ");
            await _context
                    .CreateConnection()
                    .ExecuteAsync(queryBuilder.ToString(), parameters);

            // Removed to avoid compilation issues - Izar
            // await CaptureChanges(declarant, oldDeclarant);
            return declarant;
        }

        public async Task<Declarant> SetDeleteDeclarantAsync(Guid id, bool deleted, string? LastUpdateByUser)
        {
            var parameters = new
            {
                id,
                deleted,
                LastUpdateByUser,
                LastUpdateAt = DateTime.Now,
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Declarants ");
            queryBuilder.Append("SET Deleted = @deleted ");
            if (!string.IsNullOrEmpty(LastUpdateByUser)) queryBuilder.Append(",LastUpdateByUser = @LastUpdateByUser ");
            queryBuilder.Append(",LastUpdateAt = @LastUpdateAt ");
            queryBuilder.Append(" OUTPUT INSERTED.* ");
            queryBuilder.Append(" WHERE Id = @id ");

            Declarant declarant = await _context
                .CreateConnection()
                .QuerySingleAsync<Declarant>(queryBuilder.ToString(), parameters);

            return declarant;
        }

        private async Task CaptureChanges(Declarant newDeclarant, Declarant oldDeclarant)
        {
            if (newDeclarant == null || oldDeclarant == null)
                return;

            var properties = typeof(Declarant).GetProperties();

            foreach (var property in properties)
            {
                var newValue = property.GetValue(newDeclarant);
                var oldValue = property.GetValue(oldDeclarant);

                if (!Equals(newValue, oldValue))
                {
                    var fieldName = property.Name;
                    var oldValueString = oldValue != null ? oldValue.ToString() : null;
                    var newValueString = newValue != null ? newValue.ToString() : null;

                    // Store the change in the HistoricalChanges table
                    var parameters = new
                    {
                        EntityId = newDeclarant.Id,
                        FieldName = fieldName,
                        OldValue = oldValueString,
                        NewValue = newValueString,
                        ChangedBy = newDeclarant.LastUpdateByUser,
                        ChangedDate = DateTime.Now
                    };

                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append("INSERT INTO HistoricalChanges (");
                    queryBuilder.Append("EntityId, FieldName, OldValue, NewValue, ChangedBy, ChangedDate)");
                    queryBuilder.Append(" VALUES(");
                    queryBuilder.Append("@EntityId, @FieldName, @OldValue, @NewValue, @ChangedBy, @ChangedDate)");

                    await _context.CreateConnection().ExecuteAsync(queryBuilder.ToString(), parameters);

                    //   changedFields.Add(fieldName);
                }
            }

        }

    }
}
