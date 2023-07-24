using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DocumentAnalyzerAPI.Mappers
{
    public interface IDocumentFieldsMapper
    {
        T? Map<T>(IReadOnlyDictionary<string, DocumentField> documentFields);
    }
}
