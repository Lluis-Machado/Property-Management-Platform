namespace ContactsAPI.Models
{
    public class ContactBankInformation
    {
        public string BankName { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string BIC { get; set; } = string.Empty;

        public Guid ContactPerson { get; set; }
    }
}
