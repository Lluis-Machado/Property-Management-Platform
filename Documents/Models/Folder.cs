using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DocumentsAPI.Models
{
    public class Folder : BaseModel
    {

        private static readonly string[] CONTACT_DEFAULT_FOLDERS = { "Invoices", "Personal Documents", "Others" };
        private static readonly string[] COMPANY_DEFAULT_FOLDERS = { "A/P Invoices", "A/R Invoices", "Contracts", "Others" };
        private static readonly string[] PROPERTY_DEFAULT_FOLDERS = { "Expenses", "Incomes", "Proceedings", "Others" };


        public Guid ArchiveId { get; set; }
        public string? Name { get; set; }
        public Guid? ParentId { get; set; }
        public bool HasDocument { get; set; }


        [JsonConstructor]
        public Folder() { }


        public static Folder[] CreateDefaultFolders(Guid ArchiveId, Archive.ARCHIVE_TYPE type)
        {
            Folder[] folders;
            switch (type)
            {
                case Archive.ARCHIVE_TYPE.CONTACT:
                    folders = new Folder[CONTACT_DEFAULT_FOLDERS.Length];
                    for (int i = 0; i < CONTACT_DEFAULT_FOLDERS.Length; i++)
                    {
                        folders[i] = new Folder()
                        {
                            ArchiveId = ArchiveId,
                            Name = CONTACT_DEFAULT_FOLDERS[i],
                            HasDocument = false
                        };
                    }
                    break;
                case Archive.ARCHIVE_TYPE.COMPANY:
                    folders = new Folder[COMPANY_DEFAULT_FOLDERS.Length];
                    for (int i = 0; i < COMPANY_DEFAULT_FOLDERS.Length; i++)
                    {
                        folders[i] = new Folder()
                        {
                            ArchiveId = ArchiveId,
                            Name = COMPANY_DEFAULT_FOLDERS[i],
                            HasDocument = false
                        };
                    }
                    break;
                case Archive.ARCHIVE_TYPE.PROPERTY:
                    folders = new Folder[PROPERTY_DEFAULT_FOLDERS.Length];
                    for (int i = 0; i < PROPERTY_DEFAULT_FOLDERS.Length; i++)
                    {
                        folders[i] = new Folder()
                        {
                            ArchiveId = ArchiveId,
                            Name = PROPERTY_DEFAULT_FOLDERS[i],
                            HasDocument = false
                        };
                    }
                    break;
                default:
                    folders = Array.Empty<Folder>();
                    break;
            }
            return folders;
        }


    }
}
