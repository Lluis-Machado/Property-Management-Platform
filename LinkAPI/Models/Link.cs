namespace LinkAPI.Models
{
    public class Link : BaseModel
    {
        public Guid ObjectAId { get; set; }
        public string? ObjectAType { get; set; }

        public Guid ObjectBId { get; set; }
        public string? ObjectBType { get; set; }

        public Link()
        {

        }
    }
}
