using AutoMapper;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Utilities;

namespace DocumentAnalyzerAPI.Mappers
{
    public class DocumentFieldsMapper<T> : IDocumentFieldsMapper
    {
        private readonly IMapper _mapper;
        private readonly IAPInvoiceDTOMapper _aPInvoiceDTOMapper;
        private readonly IARInvoiceDTOMapper _aRInvoiceDTOMapper;

        public DocumentFieldsMapper(IMapper mapper, IAPInvoiceDTOMapper aPInvoiceDTOMapper, IARInvoiceDTOMapper aRInvoiceDTOMapper)
        {
            _mapper = mapper;
            _aPInvoiceDTOMapper = aPInvoiceDTOMapper;
            _aRInvoiceDTOMapper = aRInvoiceDTOMapper;
        }

        public async Task<T> Map<T>(IReadOnlyDictionary<string, DocumentField> documentFields)
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
