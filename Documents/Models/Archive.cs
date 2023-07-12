using DocumentsAPI.Models;
using Newtonsoft.Json;

namespace DocumentsAPI.Models
{
    public class Archive : BaseModel
    {
        public string? Name { get; set; }

        [JsonConstructor]
        public Archive() { }

        public Archive(string pName)
        {
            Name = pName;
        }
    }
}
