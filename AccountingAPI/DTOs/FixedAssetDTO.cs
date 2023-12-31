﻿
namespace AccountingAPI.DTOs
{
    public class FixedAssetDTO
    {
        public Guid Id { get; set; }
        public Guid InvoiceLineId { get; set; }
        public string Description { get; set; }
        public DateTime CapitalizationDate { get; set; }
        public decimal AcquisitionAndProductionCosts { get; set; }
        public decimal DepreciationPercentagePerYear { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModificationBy { get; set; }

        public FixedAssetDTO()
        {
            Description = string.Empty;
            CreatedBy = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
