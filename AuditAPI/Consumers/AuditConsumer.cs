using AuditsAPI.Dtos;
using AuditsAPI.Models;
using AuditsAPI.Repositories;
using MassTransit;
using MessagingContracts;
using System.Text.Json;

namespace AuditsAPI.Consumers
{
    public class AuditConsumer : IConsumer<MessageContract>
    {
        private readonly IAuditRepository _auditRepo;

        public AuditConsumer(IAuditRepository auditRepo)
        {
            _auditRepo = auditRepo;
        }
        public async Task Consume(ConsumeContext<MessageContract> context)
        {
            var auditMessage = context.Message;

            UpsertAuditDto payload = JsonSerializer.Deserialize<UpsertAuditDto>(auditMessage.Payload)!;
            await Console.Out.WriteLineAsync(auditMessage.Payload);
            if (payload is not null)
            {
                var audit = await _auditRepo.GetByIdAsync(payload.Id);

                if (audit is not null)
                {
                    audit.Audits.Add(payload.Object);
                    await _auditRepo.UpdateAsync(audit);
                }
                else
                {
                    Audit newAudit = new();
                    newAudit.Audits.Add(payload.Object);
                    newAudit.Id = payload.Id;
                    newAudit.ObjectType = payload.ObjectType;
                    await _auditRepo.InsertOneAsync(newAudit);
                }
            }

        }
    }
}
