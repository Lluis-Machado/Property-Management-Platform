using System.Text.Json.Serialization;
using TaxManagementAPI.Models;

namespace TaxManagement.Models
{
    public class Declaration: IAuditable
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
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string? CreatedByUser { get; set; }
        public string? LastUpdateByUser { get; set; }

        [JsonConstructor]
        public Declaration() {
            CreatedByUser = String.Empty;
            LastUpdateByUser = String.Empty;
        }
    }
}
