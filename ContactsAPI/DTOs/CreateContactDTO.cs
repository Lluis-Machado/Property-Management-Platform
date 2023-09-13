using ContactsAPI.Models;

namespace ContactsAPI.DTOs
{
    public class CreateContactDto
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
        public string? Comments { get; set; }
        public string? Salutation { get; set; }

        // Multiples
        public List<ContactAddress> Addresses { get; set; } = new();
        public List<ContactPhones> Phones { get; set; } = new();
        public List<ContactIdentification> Identifications { get; set; } = new();
        public List<ContactBankInformation> BankInformation { get; set; } = new();

    }
}