namespace DocumentsAPI.DTOs
{
    public class UpdateFolderDTO
    {
        public Guid ArchiveId { get; set; }
        public string? Name { get; set; }
        public Guid? ParentId { get; set; }

    }
}
