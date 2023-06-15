namespace AccountingAPI.Models
{
    public class Period :BaseModel
    {
        public enum PeriodStatus
        {
            Open,
            Closed
        }
        public int Year { get; set; }
        public int Month { get; set; }
        public PeriodStatus Status { get; set; }
    }
}
