using Newtonsoft.Json;

namespace DocumentsAPI.Models
{
    public class Folder
    {
        public Guid Id { get; set; }
        public Guid ArchiveId { get; set; }
        public string? Name { get; set; }
        public Guid? ParentId { get; set; }
        public bool Deleted { get; set; }

        [JsonConstructor]
        public Folder() { }
    }
 }
