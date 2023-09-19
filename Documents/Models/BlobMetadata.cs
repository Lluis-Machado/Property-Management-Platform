namespace DocumentsAPI.Models
{
    public class BlobMetadata : BaseModel
    {
        public Guid BlobId { get; set; }
        public Guid ContainerId { get; set; }
        public string DisplayName { get; set; }
        public Guid? FolderId { get; set; }



    }
}
