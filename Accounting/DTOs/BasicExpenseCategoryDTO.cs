namespace AccountingAPI.DTOs
{
    public class BasicExpenseCategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ExpenseTypeCode { get; set; }

        public BasicExpenseCategoryDTO()
        {
            Name = string.Empty;
            ExpenseTypeCode = string.Empty;
        }
    }
}
