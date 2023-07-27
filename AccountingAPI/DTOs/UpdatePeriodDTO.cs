using static AccountingAPI.Utilities.PeriodStatusCodes;

namespace AccountingAPI.DTOs
{
    public class UpdatePeriodDTO
    {
        public PeriodStatus Status { get; set; }
    }
}
