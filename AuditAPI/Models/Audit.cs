namespace AuditsAPI.Models
{
    public class Audit : BaseModel
    {
        public string ObjectType { get; set; } = string.Empty;
        public List<string> Audits { get; set; } = new List<string>();
    }
}
