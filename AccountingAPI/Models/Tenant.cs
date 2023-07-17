
namespace AccountingAPI.Models
{
    public class Tenant : BaseModel
    {
        public string Name { get; set; }

        public Tenant()
        {
            Name = string.Empty;
        }
    }
}
