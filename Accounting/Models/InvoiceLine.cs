namespace Accounting.Models
{
    public class InvoiceLine
    {
        public int LineNumber { get; set; }
        public string ArticleRefNumber { get; set; }
        public string ArticleName { get; set; }
        public double Tax { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateRefFrom { get; set; }
        public DateTime DateRefTo { get; set; }
        public Guid ExpenseTypeId { get; set; }
        public Guid InvoiceId { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        public InvoiceLine()
        {
            ArticleName = string.Empty;
            ArticleRefNumber = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
