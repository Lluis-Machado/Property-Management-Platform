using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DocumentsAPI.Models
{
    public class Archive : BaseModel
    {

        public enum ARCHIVE_TYPE
        {
            CONTACT,    // 0
            COMPANY,    // 1
            PROPERTY,   // 2
            NONE        // 3
        }

        public string? Name { get; set; }

        // Serialize as string instead of INT
        [JsonConverter(typeof(StringEnumConverter))]
        public ARCHIVE_TYPE ArchiveType { get; set; }
        public Guid? RelatedItemId { get; set; }


        [JsonConstructor]
        public Archive() { }

        public Archive(string pName, Guid? relatedItemId, ARCHIVE_TYPE archiveType = ARCHIVE_TYPE.NONE)
        {
            Name = pName;
            ArchiveType = archiveType;
            RelatedItemId = relatedItemId;
        }
    }
}
