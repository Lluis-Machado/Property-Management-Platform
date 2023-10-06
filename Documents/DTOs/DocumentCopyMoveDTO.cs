namespace DocumentsAPI.DTOs
{
    public class DocumentCopyMoveDTO
    {
        public Guid destinationArchive { get; set; }
        public string documentName { get; set; }
        public Guid? folderId { get; set; }
    }
}
