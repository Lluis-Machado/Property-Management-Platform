
namespace AccountingAPI.DTOs
{
    public class CreateExpenseCategoryDTO
    {
        public string Name { get; set; }
        public string ExpenseTypeCode { get; set; }
        public int DepreciationPercent { get; set; }

        public CreateExpenseCategoryDTO()
        {
            Name = string.Empty;
            ExpenseTypeCode = string.Empty;
        }
    }
}
