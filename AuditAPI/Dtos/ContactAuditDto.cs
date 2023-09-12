namespace AuditsAPI.Dtos
{
    public class ContactAuditDto
    {
        public string? FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        //public int?[] Title { get; set; } = new int?[] { };
        public int? Gender { get; set; }
        public int MaritalStatus { get; set; }

        public DateOnly? BirthDay { get; set; }
        public string? BirthPlace { get; set; }

        // Contact Information
        public string? Email { get; set; }

        public string? Comments { get; set; }
        public string? Salutation { get; set; }

        public int Year { get; set; }
        public int Version { get; set; }

        public List<ContactAddress> Addresses { get; set; } = new();
        public List<ContactPhones> Phones { get; set; } = new();
        public List<ContactIdentification> Identifications { get; set; } = new();
        public List<ContactBankInformation> BankInformation { get; set; } = new();
        public class ContactAddress
        {
            public string? AddressLine1 { get; set; }
            public string? AddressLine2 { get; set; }
            public string? City { get; set; }
            public int? State { get; set; }
            public string? PostalCode { get; set; }
            public string? ShortComment { get; set; }

            public int? Country { get; set; }
            public int? AddressType { get; set; }
        }
        public class ContactBankInformation
        {
            public string BankName { get; set; } = string.Empty;
            public string IBAN { get; set; } = string.Empty;
            public string BIC { get; set; } = string.Empty;

            public Guid ContactPerson { get; set; }
        }
        public class ContactPhones
        {
            public int PhoneType { get; set; }
            public int Type { get; set; }
            public int CountryMaskId { get; set; }
            public string PhoneNumber { get; set; } = string.Empty;
            public string ShortComment { get; set; } = string.Empty;


        }

        public class ContactIdentification
        {
            public int Type { get; set; }
            public string Number { get; set; } = string.Empty;

            public DateOnly? EmissionDate { get; set; }
            public DateOnly? ExpirationDate { get; set; }
            public string ShortComment { get; set; } = string.Empty;


        }

        public DateTime LastUpdateAt { get; set; }
        public string? LastUpdateByUser { get; set; }

    }
}
