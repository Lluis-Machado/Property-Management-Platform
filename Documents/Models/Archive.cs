using Newtonsoft.Json;

namespace Documents.Models
{
    public class Archive
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        [JsonConstructor]
        public Archive() { }

        public Archive(string pName) {
            Name = pName;
        }
    }
}
