namespace Documents.Models
{
    public interface IDocumentName
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Extension { get; set; }
    }
}
