namespace AuditsAPI.Dtos
{
    public class CompanyAuditDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Nif { get; set; }
        public string? GermanTaxOffice { get; set; }
        public string? CompanyPurpose { get; set; }
        public string? TaxNumber { get; set; }
        public string? UStIDNumber { get; set; }
        public DateOnly? FoundingDate { get; set; }


        // Contact Information
        public string? Email { get; set; }
        public int? CountryMaskId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Comments { get; set; }
        

        public int Version { get; set; }

        public DateTime LastUpdateAt { get; set; }
        public string? LastUpdateByUser { get; set; }

    }
}
