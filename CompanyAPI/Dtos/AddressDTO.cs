namespace CompanyAPI.Dtos
{
    public class AddressDto
    {
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public int? State { get; set; } = null;
        public string? PostalCode { get; set; }
        public int? Country { get; set; } = null;
        public int? AddressType { get; set; } = null;
        public string? ShortComment { get; set; }
        public string? FullAddress
        {
            set
            {
                // Combine the address components into a full address string
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var components = new List<string>();
                    if (!string.IsNullOrWhiteSpace(AddressLine1))
                        components.Add(AddressLine1);
                    if (!string.IsNullOrWhiteSpace(AddressLine2))
                        components.Add(AddressLine2);
                    if (!string.IsNullOrWhiteSpace(City))
                        components.Add(City);
                    if (!string.IsNullOrWhiteSpace(PostalCode))
                        components.Add(PostalCode);

                    value = string.Join(", ", components);
                }
            }
        }
    }
}
