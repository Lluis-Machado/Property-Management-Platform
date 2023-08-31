namespace AuditsAPI.Dtos
{
    public class ContactAuditDto
    {
        public string Name { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;

        public int Year { get; set; }
        public int Version { get; set; }

        public DateTime LastUpdateAt { get; set; }
        public string? LastUpdateByUser { get; set; }

    }
}
