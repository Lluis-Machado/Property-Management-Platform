namespace PropertyManagementAPI.Models
{
    public class PropertyManagentDataBaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string PropertyCollectionName { get; set; } = null!;
    }
}
