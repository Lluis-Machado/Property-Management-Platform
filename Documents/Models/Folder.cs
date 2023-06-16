using Newtonsoft.Json;

namespace DocumentsAPI.Models
{
    public class Folder : BaseModel
    {
        public Guid ArchiveId { get; set; }
        public string? Name { get; set; }
        public Guid? ParentId { get; set; }

        [JsonConstructor]
        public Folder() { }
    }
 }
