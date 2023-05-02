namespace Documents.Models
{
    public class DocumentName : IDocumentName
    {
        private readonly int _idNbOfChars = 36;
        public string Name { get; }
        public string Code { get; }
        public string DocName { get; set; }
        public string Extension { get; }

        public DocumentName(string pName)
        {
            Name = pName;
            Code = pName.Length >= _idNbOfChars? pName[.._idNbOfChars] : "";
            DocName = pName.Length >= _idNbOfChars + 1? pName[(_idNbOfChars + 1)..] : "";
            Extension = pName.Contains('.') ? pName[pName.LastIndexOf('.')..] : "";
        }
    }
}
