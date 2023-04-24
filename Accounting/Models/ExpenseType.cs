namespace Accounting.Models
{
    public class ExpenseType
    {
        public int Code { get; set; }
        public string Description { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        public ExpenseType() 
        {
            Description = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
