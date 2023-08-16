namespace ContactsAPI.Models
{
    public class ContactIdentification
    {
        public int Type { get; set; }
        public string Number { get; set; } = string.Empty;

        public DateOnly? EmissionDate { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public string ShortComment { get; set; } = string.Empty;


    }
}
