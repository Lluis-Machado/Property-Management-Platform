namespace AuditsAPI.Dtos
{
    public class CompanyAuditDto
    {
        // Company Information
        public string Name { get; set; } = string.Empty;
        public string? Nif { get; set; }
        public string? GermanTaxOffice { get; set; }
        public string? CompanyPurpose { get; set; }
        public string? UStIDNumber { get; set; }
        public DateOnly? FoundingDate { get; set; }


        // Contact Information
        public string? Email { get; set; }
        public int? CountryMaskId { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Comments { get; set; }

        // Lists
        public List<CompanyAddress> Addresses { get; set; } = new();
        public List<CompanyContact> Contacts { get; set; } = new();
        public List<CompanyBankInformation> BankInformation { get; set; } = new();

        public class CompanyContact
        {
            public Guid ContactId { get; set; }
            public int? ContactType { get; set; }
            public string? ShortComment { get; set; }

        }
        public class CompanyBankInformation
        {
            public string BankName { get; set; } = string.Empty;
            public string IBAN { get; set; } = string.Empty;
            public string BIC { get; set; } = string.Empty;

            public Guid ContactPerson { get; set; }
        }
        public class CompanyAddress
        {
            public string? AddressLine1 { get; set; }
            public string? AddressLine2 { get; set; }
            public string? City { get; set; }
            public int? State { get; set; }
            public string? PostalCode { get; set; }
            public int? Country { get; set; }
            public int? AddressType { get; set; }
            public string? ShortComment { get; set; }

        }

        public int Version { get; set; }

        public DateTime LastUpdateAt { get; set; }
        public string? LastUpdateByUser { get; set; }

    }
}
