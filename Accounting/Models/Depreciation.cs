using System.Text.Json.Serialization;

namespace AccountingAPI.Models
{
    public class Depreciation :BaseModel
    {
        public Guid FixedAssetId { get; set; }
        public Guid PeriodId  { get; set; }
        public double DepreciationAmount { get; set; }

        [JsonConstructor]
        public Depreciation()
        {
        }
    }
}
