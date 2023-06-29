namespace AccountingAPI.DTOs
{
    public class UpdateTenantDTO
    {
        public string Name { get; set; }

        public UpdateTenantDTO()
        {
            Name = string.Empty;
        }
    }
}
