namespace DocumentsAPI.DTOs
{
    public class UpdateFolderDTO
    {
        public Guid ArchiveId { get; set; }
        public string? Name { get; set; }
        public Guid? ParentId { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; } = false;

    }
}
