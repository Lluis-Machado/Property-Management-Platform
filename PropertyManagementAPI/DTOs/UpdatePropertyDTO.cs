using PropertiesAPI.Models;

namespace PropertiesAPI.Dtos
{
    public class UpdatePropertyDto
    {
        public string? Name { get; set; }
        public int? Type { get; set; }
        public List<int> TypeOfUse { get; set; } = new();

        // Cadastre Information
        public string? CadastreNumber { get; set; }
        public string? CadastreRef { get; set; }
        public string? CadastreValue { get; set; }
        public string? AutonomousRegion { get; set; }
        public string? FederalState { get; set; }
        public Price? LoanPrice { get; set; }
        public Price? BuildingPrice { get; set; }
        public Price? TotalPrice { get; set; }
        public Price? PlotPrice { get; set; }
        public string? IBIAmount { get; set; }
        public int? IBICollection { get; set; }
        public int? Year { get; set; }
        public string? Municipality { get; set; }
        public string? PropertyScanMail { get; set; }

        // Purchase Information
        public DateOnly? PurchaseDate { get; set; }
        public Price? PurchasePriceAJD { get; set; }
        public Price? PurchasePriceTotal { get; set; }
        public Price? PurchasePriceTax { get; set; }
        public Price? PurchasePriceTPO { get; set; }
        public Price? PurchasePrice { get; set; }

        // Furniture Information
        public Price? FurniturePriceIVA { get; set; }
        public Price? FurniturePriceTPO { get; set; }
        public Price? FurniturePrice { get; set; }
        public int? GarbageCollection { get; set; }
        public int? GarbagePriceAmount { get; set; }

        // Sale Information
        public DateOnly? SaleDate { get; set; }
        public Price? SalePrice { get; set; }

        // Other Information
        public int? BedNumber { get; set; }
        public string? Comments { get; set; }

        // Address Information
        public List<PropertyAddress> PropertyAddress { get; set; } = new List<PropertyAddress>();

        // Identifiers
        public Guid? MainPropertyId { get; set; }
        public Guid ContactPersonId { get; set; }
        public Guid MainOwnerId { get; set; }

        public Guid BillingContactId { get; set; }

    }
}
