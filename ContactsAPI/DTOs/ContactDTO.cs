namespace ContactsAPI.DTOs
{
    public class ContactDTO
    {
        // Personal Information
        public string? FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public int?[] Title { get; set; } = new int?[] { };
        public int? Gender { get; set; }
        public int MaritalStatus { get; set; }

        public DateOnly? BirthDay { get; set; }
        public string? BirthPlace { get; set; }

        // Contact Information
        public string? Email { get; set; }
        // Bank Information

        public string? Comments { get; set; }
        public string? Salutation { get; set; }


        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public Guid? ArchiveId { get; set; }

    }
}