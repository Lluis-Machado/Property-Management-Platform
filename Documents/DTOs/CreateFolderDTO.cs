namespace DocumentsAPI.DTOs
{
    public class CreateFolderDTO
    {
        public Guid ArchiveId { get; set; }
        public string? Name { get; set; }
        public Guid? ParentId { get; set; }
    }
}
