
namespace AccountingAPI.Models
{
    public class Loan : BaseModel
    {
        public Guid BusinessPartnerId { get; set; }
        public string Concept { get; set; }
        public decimal Amount { get; set; }

        public Loan()
        {
            Concept = string.Empty;
        }
    }
}
