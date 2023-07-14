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

        public Folder GetTopLevelFolder()
        {
            return new Folder
            {
                Id = Id,
                ArchiveId = ArchiveId,
                Name = Name,
                ParentId = ParentId,
                Deleted = Deleted
            };
        }

        public List<Folder> GetChildrenAsFolders()
        {
            List<Folder> result = new();
            foreach (var child in ChildFolders)
            {
                result.Add(child.GetTopLevelFolder());
            }
            return result;
        }
    }
}
