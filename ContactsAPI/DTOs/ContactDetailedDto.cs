using ContactsAPI.Models;

namespace ContactsAPI.DTOs
{
    public class ContactDetailedDto : ContactDTO
    {
        // Multiples
        public List<ContactAddress> Addresses { get; set; } = new();
        public List<ContactPhones> Phones { get; set; } = new();
        public List<ContactIdentification> Identifications { get; set; } = new();
        public List<ContactBankInformation> BankInformation { get; set; } = new();

    }
}
