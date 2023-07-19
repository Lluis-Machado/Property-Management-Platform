namespace CompanyAPI.Models
{
    public class Company : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Nif { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public CompanyAddress Address{ get; set; } = new CompanyAddress();

    }
}
