using System.Net;

namespace DocumentsAPI.DTOs
{
    public class DocumentUploadDTO
    {
        public Guid documentId { get; set; }
        public HttpStatusCode statusCode { get; set; }
    }
}
