namespace ContactsAPI.Models
{
    public class Contact : BaseModel
    {
        // Personal Information
        public string? FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public int? Title { get; set; }
        public int? Gender { get; set; }
        public int MaritalStatus { get; set; }

        public DateOnly? BirthDay { get; set; }
        public string? BirthPlace { get; set; }

        // Contact Information
        public string? Email { get; set; }
        // Bank Information
        public string? IBAN { get; set; }


        // Multiples
        public List<ContactAddress> Addresses { get; set; } = new();
        public List<ContactPhones> Phones { get; set; } = new();
        public List<ContactIdentification> Identifications { get; set; } = new();
        public List<ContactBankInformation> BankInformation { get; set; } = new();

    }
}
