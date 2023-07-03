namespace DocumentsAPI.DTOs
{
    public class FolderStructureDTO
    {
        public Guid Id { get; set; }
        public List<FolderStructureDTO> Folders { get; set; } = new List<FolderStructureDTO>();
    }
}
