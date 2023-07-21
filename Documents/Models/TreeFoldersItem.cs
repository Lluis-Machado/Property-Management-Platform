using DocumentsAPI.Repositories;

namespace DocumentsAPI.Models
{
    public class TreeFolderItem : Folder
    {

        public List<TreeFolderItem> ChildFolders { get; set; }

        public TreeFolderItem(Folder folder, IFolderRepository? repository = null)
        {
            Id = folder.Id;
            ArchiveId = folder.ArchiveId;
            Name = folder.Name;
            ParentId = folder.ParentId;
            Deleted = folder.Deleted;
            CreatedAt = folder.CreatedAt;
            CreatedByUser = folder.CreatedByUser;
            LastUpdateAt = folder.LastUpdateAt;
            LastUpdateByUser = folder.LastUpdateByUser;
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
                Deleted = Deleted,
                CreatedAt = CreatedAt,
                CreatedByUser = CreatedByUser,
                LastUpdateAt = LastUpdateAt,
                LastUpdateByUser = LastUpdateByUser,
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
