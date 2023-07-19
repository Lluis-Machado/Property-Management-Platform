using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DocumentAnalyzerAPI.Mappers
{
    public interface IDocumentFieldsMapper
    {
        Task<T> Map<T>(IReadOnlyDictionary<string, DocumentField> documentFields);
    }
}
