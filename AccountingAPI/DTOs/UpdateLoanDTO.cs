
namespace AccountingAPI.DTOs
{
    public class UpdateLoanDTO
    {
        public string Concept { get; set; }
        public decimal Amount { get; set; }

        public UpdateLoanDTO()
        {
            Concept = string.Empty;
        }
    }
}
