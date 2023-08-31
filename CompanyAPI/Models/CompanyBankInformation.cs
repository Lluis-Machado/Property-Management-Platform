namespace CompanyAPI.Models
{
    public class CompanyBankInformation
    {
        public string BankName { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string BIC { get; set; } = string.Empty;

        public Guid ContactPerson { get; set; }
    }
}
