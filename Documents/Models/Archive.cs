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

        // The archive Name is the display_name field in the metadata.
        // The name of the container itself (FullArchiveId) will have this format:
        /**
         * For CONTACT archives:    ArchiveId = Contact.Id; FullArchiveId = "cont_{Contact.Id}"
         * For COMPANY archives:    ArchiveId = Company.Id; FullArchiveId = "comp_{Company.Id}"
         * For PROPERTY archives:   ArchiveId = Property.Id; FullArchiveId = "prop_{Contact.Id}"
         * For archives not associated with any other object: ArchiveId = new Guid(); FullArchiveId = "generic_{ArchiveId}"
         */


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
