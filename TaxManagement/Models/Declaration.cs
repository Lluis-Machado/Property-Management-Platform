using System.Text.Json.Serialization;
using TaxManagementAPI.Models;

namespace TaxManagement.Models
{
    public class Declaration: BaseModel
    {
        public Guid DeclarantId { get; set; }
        public DeclarationStatus Status { get; set; }

        public GenericModel? ModelPresented { get; set; }

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
        [JsonConstructor]
        public Declaration() {
            CreatedByUser = String.Empty;
            LastUpdateByUser = String.Empty;
        }
    }
}
