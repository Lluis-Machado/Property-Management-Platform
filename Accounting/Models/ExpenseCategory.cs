using AccountingAPI.Models;

namespace AccountingAPI.Models
{
    public enum ExpenseType
    {
        UAT,
        UAV,
        BAT,
        BAV,
        Asset
    }

    public class ExpenseCategory :BaseModel
    {
        public string? Name { get; set; }
        public string? ExpenseTypeCode { get; set; }
        public int DepreciationPercent { get; set; }
    }
}
