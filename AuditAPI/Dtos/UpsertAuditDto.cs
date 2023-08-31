namespace AuditsAPI.Dtos
{
    public class UpsertAuditDto
    {
        public Guid Id { get; set; }
        public string ObjectType { get; set; } = string.Empty;
        public string Object { get; set; } = string.Empty;
        public int Version { get; set; } = 0;

    }
}
