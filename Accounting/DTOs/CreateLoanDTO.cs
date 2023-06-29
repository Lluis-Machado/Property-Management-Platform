
namespace AccountingAPI.DTOs
{
    public class CreateLoanDTO
    {
        public string Concept { get; set; }
        public decimal Amount { get; set; }

        public CreateLoanDTO()
        {
            Concept = string.Empty;
        }
    }
}
