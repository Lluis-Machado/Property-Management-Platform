using System.Text.Json.Serialization;

namespace TaxManagement.Models
{
    public class Declaration
    {
        public enum DeclarationStatus
        {
            WaitingForVerification,
            Verified,
            WaitingForValidation,
            Validated,
            Closed = 10,
            VerificationRejected = -1,
            ValidationRejected = -2
        }
        public Guid Id { get; set; }
        public Guid DeclarantId { get; set; }
        public DeclarationStatus Status { get; set; }
        public bool Deleted { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }

        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }

        [JsonConstructor]
        public Declaration() {
            CreateUser = String.Empty;
            UpdateUser = String.Empty;
        }
    }
}
