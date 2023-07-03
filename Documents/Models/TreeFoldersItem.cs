namespace DocumentsAPI.Models
{
    public class TreeFolderItem : Folder
    {
        public List<TreeFolderItem> ChildFolders { get; set; }

        public TreeFolderItem(Folder folder)
        {
            Id = folder.Id;
            ArchiveId = folder.ArchiveId;
            Name = folder.Name;
            ParentId = folder.ParentId;
            Deleted = folder.Deleted;
            ChildFolders = new List<TreeFolderItem>();
        }
    }
}
