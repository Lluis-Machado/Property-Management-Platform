namespace AccountingAPI.DTOs
{
    public class UpdatePeriodDTO
    {
        public enum PeriodStatus
        {
            open,
            closed
        }
        public PeriodStatus Status { get; set; }
    }
}
