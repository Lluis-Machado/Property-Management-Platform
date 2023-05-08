namespace PropertyManagementAPI.Models
{
    public class Property
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AccoutingTenantId { get; set; }
        public Guid DocumentsTenantId { get; set; }
        public Property()
        {
            Name = String.Empty;
        }
    }
}
