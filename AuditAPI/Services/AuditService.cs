using AuditsAPI.Dtos;
using AuditsAPI.Models;
using AuditsAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using System.Text.Json;

namespace AuditsAPI.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepo;

        public AuditService(IAuditRepository auditRepo)
        {
            _auditRepo = auditRepo;
        }

        public async Task<ActionResult<IEnumerable<Audit>>> GetAsync(bool includeDeleted = false)
        {
            var audit = await _auditRepo.GetAsync(includeDeleted);

            return new OkObjectResult(audit);
        }

        public async Task<List<Dictionary<string, Tuple<object, object>>>> GetByIdAsync(Guid id)
        {
            var audit = await _auditRepo.GetByIdAsync(id);
            AuditOutDto outDto = new AuditOutDto();
            outDto.Audits = audit.Audits;

            List<PropertyAuditDto> audits = new();

            foreach (string serializedString in audit.Audits)
            {
                var obj = JsonSerializer.Deserialize<PropertyAuditDto>(serializedString);
                audits.Add(obj!);
            }
            List<Dictionary<string, Tuple<object, object>>> differences = new();
            if (audits.Count>1)
            { 
                for (int i = 0; i < audits.Count-1; i++)
                {
                    audits[i].Version = i + 1;
                    audits[i+1].Version = i + 2;
                    differences.Add(ObjectComparer.CompareObjects(audits[i], audits[i+1]));
                }
            }
            //
            return differences;
        }

        public async Task<List<Dictionary<string, Tuple<object, object>>>> GetByPropertyIdAsync(Guid id)
        {
            var audit = await _auditRepo.GetByIdAsync(id);
            AuditOutDto outDto = new AuditOutDto();
            outDto.Audits = audit.Audits;

            List<PropertyAuditDto> audits = new();

            foreach (string serializedString in audit.Audits)
            {
                var obj = JsonSerializer.Deserialize<PropertyAuditDto>(serializedString);
                audits.Add(obj!);
            }
            List<Dictionary<string, Tuple<object, object>>> differences = new();
            if (audits.Count > 1)
            {
                for (int i = 0; i < audits.Count - 1; i++)
                {
                    audits[i].Version = i + 1;
                    audits[i + 1].Version = i + 2;
                    differences.Add(ObjectComparer.CompareObjects(audits[i], audits[i + 1]));
                }
            }
            //c431878d-c6f8-4fc8-960d-654f9c262064
            return differences;
        }
        public async Task<List<Dictionary<string, Tuple<object, object>>>> GetByContactIdAsync(Guid id)
        {
            var audit = await _auditRepo.GetByIdAsync(id);
            AuditOutDto outDto = new AuditOutDto();
            outDto.Audits = audit.Audits;

            List<ContactAuditDto> audits = new();

            foreach (string serializedString in audit.Audits)
            {
                var obj = JsonSerializer.Deserialize<ContactAuditDto>(serializedString);
                audits.Add(obj!);
            }
            List<Dictionary<string, Tuple<object, object>>> differences = new();
            if (audits.Count > 1)
            {
                for (int i = 0; i < audits.Count - 1; i++)
                {
                    audits[i].Version = i + 1;
                    audits[i + 1].Version = i + 2;
                    differences.Add(ObjectComparer.CompareObjects(audits[i], audits[i + 1]));
                }
            }
            //c431878d-c6f8-4fc8-960d-654f9c262064
            return differences;
        }
        public async Task<List<Dictionary<string, Tuple<object, object>>>> GetByCompanyIdAsync(Guid id)
        {
            var audit = await _auditRepo.GetByIdAsync(id);
            AuditOutDto outDto = new AuditOutDto();
            outDto.Audits = audit.Audits;

            List<CompanyAuditDto> audits = new();

            foreach (string serializedString in audit.Audits)
            {
                var obj = JsonSerializer.Deserialize<CompanyAuditDto>(serializedString);
                audits.Add(obj!);
            }
            List<Dictionary<string, Tuple<object, object>>> differences = new();
            if (audits.Count > 1)
            {
                for (int i = 0; i < audits.Count - 1; i++)
                {
                    audits[i].Version = i + 1;
                    audits[i + 1].Version = i + 2;
                    differences.Add(ObjectComparer.CompareObjects(audits[i], audits[i + 1]));
                }
            }
            //c431878d-c6f8-4fc8-960d-654f9c262064
            return differences;
        }




        public async Task<IActionResult> DeleteAsync(Guid companyId, string lastUser)
        {
            var updateResult = await _auditRepo.SetDeleteAsync(companyId, true, lastUser);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Company not found");

            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteAsync(Guid companyId, string lastUser)
        {
            var updateResult = await _auditRepo.SetDeleteAsync(companyId, false, lastUser);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Company not found");

            return new NoContentResult();
        }
    }
}
