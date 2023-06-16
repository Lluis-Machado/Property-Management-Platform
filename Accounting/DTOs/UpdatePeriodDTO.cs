namespace AccountingAPI.DTOs
{
    public class UpdatePeriodDTO
    {
        public enum PeriodStatus
        {
            Open,
            Closed
        }
        public PeriodStatus Status { get; set; }
    }
}
