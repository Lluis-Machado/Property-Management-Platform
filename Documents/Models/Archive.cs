using Newtonsoft.Json;

namespace DocumentsAPI.Models
{
    public class Archive : BaseModel
    {

        public enum ARCHIVE_TYPE
        {
            CONTACT,
            COMPANY,
            PROPERTY,
            NONE
        }

        public string? Name { get; set; }
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
