using System.Text.Json.Serialization;

namespace DocumentsAPI.Models
{
    public class Document : BaseModel
    {
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public long? ContentLength { get; set; }
        public Guid? FolderId { get; set; }

        [JsonConstructor]
        public Document() { }
    }
}
