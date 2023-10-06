using System.Net;

namespace DocumentsAPI.Models
{

    public class CreateDocumentStatus
    {
        public string FileName { get; set; }
        public int Status { get; set; }

        public CreateDocumentStatus(string fileName, int status)
        {
            FileName = fileName;
            Status = status;
        }
    }
}
