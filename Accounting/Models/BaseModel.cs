namespace AccountingAPI.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModificationBy { get; set; }

        public BaseModel()
        {
            CreatedBy = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
