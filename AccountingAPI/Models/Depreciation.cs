﻿
namespace AccountingAPI.Models
{
    public class Depreciation : BaseModel
    {
        public Guid FixedAssetId { get; set; }
        public Guid PeriodId { get; set; }
        public decimal DepreciationAmount { get; set; }
    }
}
