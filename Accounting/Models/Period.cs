namespace AccountingAPI.Models
{
    public class Period
    {
        public enum PeriodStatus
        {
            Open,
            Closed
        }
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public PeriodStatus Status { get; set; }
    }
}
