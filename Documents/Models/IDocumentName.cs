namespace Documents.Models
{
    public interface IDocumentName
    {
        public string? Code { get; }
        public string? Name { get; }
        public string? Extension { get; }
    }
}
