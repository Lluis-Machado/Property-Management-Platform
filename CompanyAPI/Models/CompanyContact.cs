namespace CompanyAPI.Models
{
    public class CompanyContact
    {
        public Guid ContactId { get; set; }
        public int? ContactType { get; set; }
        public string? ShortComment { get; set; }

    }
}
