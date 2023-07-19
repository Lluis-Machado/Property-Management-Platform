using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Mappers
{
    public class DocumentFieldsMapper : IDocumentFieldsMapper
    {
        private readonly IAPInvoiceDTOMapper _aPInvoiceDTOMapper;
        private readonly IARInvoiceDTOMapper _aRInvoiceDTOMapper;

        public DocumentFieldsMapper(IAPInvoiceDTOMapper aPInvoiceDTOMapper, IARInvoiceDTOMapper aRInvoiceDTOMapper)
        {
            _aPInvoiceDTOMapper = aPInvoiceDTOMapper;
            _aRInvoiceDTOMapper = aRInvoiceDTOMapper;
        }

        public T Map<T>(IReadOnlyDictionary<string, DocumentField> documentFields)
        {
            Type targetType = typeof(T);

            // Determine the mapping method based on T
            Func<IReadOnlyDictionary<string, DocumentField>, object> mappingMethod;
            if (targetType == typeof(APInvoiceDTO))
            {
                mappingMethod = _aPInvoiceDTOMapper.MapToAPInvoiceAndLinesDTO;
            }
            else if(targetType == typeof(ARInvoiceDTO))
            {
                mappingMethod = _aRInvoiceDTOMapper.MapToARInvoiceAndLinesDTO;
            }
            else
            {
                // Handle unsupported types here
                throw new NotSupportedException($"Mapping for type {targetType.Name} is not supported.");
            }

            object result = mappingMethod(documentFields);

            return result is T mappedObject ? mappedObject : default;
        }

    }
}
