﻿
namespace AccountingAPI.DTOs
{
    public class CreateFixedAssetDTO
    {
        public Guid InvoiceLineId { get; set; }
        public string Description { get; set; }
        public DateTime CapitalizationDate { get; set; }
        public double AcquisitionAndProductionCosts { get; set; }
        public double DepreciationPercentagePerYear { get; set; }
    }
}
