﻿using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class PeriodRepository : IPeriodRepository
    {
        private readonly IDapperContext _context;
        public PeriodRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<Period> InsertPeriodAsync(Period period)
        {
            var parameters = new
            {
                period.TenantId,
                period.Year,
                period.Month,
                period.CreatedBy,
                period.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Periods (");
            queryBuilder.Append("TenantId");
            queryBuilder.Append(",Year");
            queryBuilder.Append(",Month");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.TenantId");
            queryBuilder.Append(",INSERTED.Year");
            queryBuilder.Append(",INSERTED.Month");
            queryBuilder.Append(",INSERTED.Status");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append("@TenantId");
            queryBuilder.Append(",@Year");
            queryBuilder.Append(",@Month");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(")");

            return await _context.Connection.QuerySingleAsync<Period>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Period>> GetPeriodsAsync(Guid tenantId, bool includeDeleted = false)
        {
            var parameters = new
            {
                tenantId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",TenantId");
            queryBuilder.Append(",Year");
            queryBuilder.Append(",Month");
            queryBuilder.Append(",Status");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM Periods");
            queryBuilder.Append(" WHERE TenantId = @tenantId");
            if (!includeDeleted) queryBuilder.Append(" AND Deleted = 0");

            return await _context.Connection.QueryAsync<Period>(queryBuilder.ToString(), parameters);
        }

        public async Task<Period> GetPeriodByIdAsync(Guid periodId)
        {
            var parameters = new
            {
                periodId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",TenantId");
            queryBuilder.Append(",Year");
            queryBuilder.Append(",Month");
            queryBuilder.Append(",Status");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM Periods");
            queryBuilder.Append(" WHERE Id = @periodId");

            return await _context.Connection.QuerySingleAsync<Period>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedPeriodAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Periods");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            return await _context.Connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<Period> UpdatePeriodAsync(Period period)
        {
            var parameters = new
            {
                period.Id,
                period.Status,
                period.LastModificationBy,
                period.LastModificationAt,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Periods");
            queryBuilder.Append(" SET Status = @Status");
            queryBuilder.Append(",LastModificationAt = @LastModificationAt");
            queryBuilder.Append(",LastModificationBy = @LastModificationBy");
            queryBuilder.Append(" OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.TenantId");
            queryBuilder.Append(",INSERTED.Year");
            queryBuilder.Append(",INSERTED.Month");
            queryBuilder.Append(",INSERTED.Status");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context.Connection.QuerySingleAsync<Period>(queryBuilder.ToString(), parameters);
        }
    }
}
