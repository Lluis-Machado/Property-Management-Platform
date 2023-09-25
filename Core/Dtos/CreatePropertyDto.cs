namespace CoreAPI.Dtos
{
    public class CreatePropertyDto
    {
        // Property Information
        public string? Name { get; set; }

        public int? BedNumber { get; set; }
        public string? Comments { get; set; }

        // Address Information
        public PropertyAddress PropertyAddress { get; set; } = new();

        // Identifiers
        public Guid? MainPropertyId { get; set; }
        public Guid? ContactPersonId { get; set; }
        public Guid MainOwnerId { get; set; }
        public string MainOwnerType { get; set; } = "Contact";

        public Guid? BillingContactId { get; set; }



    }

    public class PropertyAddress
    {
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public int? State { get; set; }
        public string? PostalCode { get; set; }
        public int Country { get; set; } = 1;
        public int? AddressType { get; set; }

    }

    public class Price
    {
        public string Currency { get; set; } = "€";
        public decimal Value { get; set; }
    }
}
