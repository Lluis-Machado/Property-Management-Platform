
namespace AccountingAPI.DTOs
{
    public class UpdateExpenseCategoryDTO
    {
        public string? Name { get; set; }
        public string? ExpenseTypeCode { get; set; }
        public int DepreciationPercent { get; set; }
    }
}
