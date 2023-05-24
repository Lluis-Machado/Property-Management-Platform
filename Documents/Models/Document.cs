using System.Text.Json.Serialization;

namespace Documents.Models
{
    public class Document
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public long? ContentLength { get; set; }
        public Guid? FolderId  { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? LastModified { get; set; }

        [JsonConstructor]
        public Document() { }
    }
}
