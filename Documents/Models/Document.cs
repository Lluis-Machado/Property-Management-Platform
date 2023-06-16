using DocumentsAPI.Models;
using System.Text.Json.Serialization;

namespace Documents.Models
{
    public class Document : BaseModel
    {
        public string DocumentId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public long? ContentLength { get; set; }
        public Guid? FolderId  { get; set; }

        [JsonConstructor]
        public Document() { }
    }
}
