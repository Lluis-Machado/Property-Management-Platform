namespace DocumentsAPI.DTOs
{
    public class FolderDTO
    {
        public Guid ArchiveId { get; set; }
        public string? Name { get; set; }
        public Guid? ParentId { get; set; }
        public bool HasDocument { get; set; }
        List<FolderDTO>? Subfolders { get; set; }

        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string? CreatedByUser { get; set; }
        public string? LastUpdateByUser { get; set; }
        public bool Deleted { get; set; } = false;


    }
}
