using static AccountingAPI.Utilities.PeriodStatusCodes;

namespace AccountingAPI.Models
{
    public class Period : BaseModel
    {
        public Guid TenantId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public PeriodStatus Status { get; set; }
    }
}
