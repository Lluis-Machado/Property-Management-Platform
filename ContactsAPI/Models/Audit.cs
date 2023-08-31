namespace ContactsAPI.Models
{
    public class Audit : BaseModel
    {
        public string ObjectType { get; set; } = "";

        public string Object { get; set; } = "";
    }
}
