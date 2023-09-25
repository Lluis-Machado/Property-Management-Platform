namespace ContactsAPI.Models
{
    public class ContactBankInformation
    {
        public string BankName { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string BIC { get; set; } = string.Empty;

        public string ContactName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
    }
}
