namespace Documents.Models
{
    public class DocumentName : IDocumentName
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Extension { get; set; }
    }
}
