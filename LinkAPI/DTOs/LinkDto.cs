namespace LinkAPI.Dtos
{
    public class LinkDto 
    {
        public Guid Id { get; set; }

        public Guid ObjectAId { get; set; }
        public string ObjectAType { get; set; }

        public Guid ObjectBId { get; set; }
        public string ObjectBType { get; set; }
        public bool Deleted { get; set; } = false;

        
        public LinkDto() { }
    }
}
