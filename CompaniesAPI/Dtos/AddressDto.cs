namespace CompaniesAPI.Dtos
{
    public class AddressDto
    {
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public int State { get; set; }
        public string? PostalCode { get; set; }
        public int Country { get; set; }
        public bool DefaultAddress { get; set; }
        public string? AddressType { get; set; }
    }
}