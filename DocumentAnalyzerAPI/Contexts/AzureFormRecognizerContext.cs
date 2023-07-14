using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DocumentAnalyzerAPI.Contexts
{
    public class AzureFormRecognizerContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _key;
        public AzureFormRecognizerContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = $"https://{_configuration.GetValue<string>("AzureFormRecognizer:FormRecognizerAccount")}.cognitiveservices.azure.com";
            _key = _configuration.GetValue<string>("AzureFormRecognizer:Key");
        }

        public DocumentAnalysisClient GetDocumentAnalysisClient()
        {
            AzureKeyCredential credential = new(_key);
            return new(new Uri(_connectionString), credential);
        }
    }
}
