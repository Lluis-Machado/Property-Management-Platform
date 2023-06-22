using DocumentsAPI.Models;

namespace DocumentsAPI.DTOs
{
    public class FolderStructreDTO
    {
        public Guid Id { get; set; }
        public List<FolderStructreDTO> Folders { get; set; } = new List<FolderStructreDTO>();
    }
}
