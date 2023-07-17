using AutoMapper;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Mappers
{
    public class APInvoiceDTOMapper : IAPInvoiceDTOMapper
    {
        private readonly IMapper _mapper;

        public APInvoiceDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public APInvoiceDTO MapToAPInvoiceDTO(IReadOnlyDictionary<string, DocumentField> documentFields)
        {
            APInvoiceDTO aPInvoiceDTO = new()
            {
                RefNumber = MapFieldInfo<string>(documentFields,"InvoiceId"),
                Date = MapFieldInfo<DateTime>(documentFields,"InvoiceDate"),
                Currency = MapFieldInfo<string>(documentFields, "Currency")
            };
            // Map other properties as needed

            return aPInvoiceDTO;
        }

        private FieldInfo<T> MapFieldInfo<T>(IReadOnlyDictionary<string, DocumentField> documentFields, string fieldName)
        {
            FieldInfo<T> fieldInfo = new();

            if (!documentFields.TryGetValue(fieldName, out DocumentField documentField)) return fieldInfo;

            fieldInfo.Value = GetFieldValue<T>(documentField);
            fieldInfo.Confidence = documentField.Confidence;
            fieldInfo.BoundingRegions = _mapper.Map<List<BoundingRegionDTO>>(documentField.BoundingRegions);

            return fieldInfo;
        }

        private T GetFieldValue<T>(DocumentField documentField)
        {
            switch (documentField.FieldType)
            {
                case DocumentFieldType.String:
                    return documentField.Value.AsString() is T strValue ? strValue : default;
                case DocumentFieldType.Date:
                    return documentField.Value.AsDate() is T dateValue ? dateValue : default;
                case DocumentFieldType.Double:
                    return documentField.Value.AsDouble() is T doubleValue ? doubleValue : default;
                case DocumentFieldType.Currency:
                    if (documentField.Value.AsCurrency().Amount is T currencyValue)
                    {
                        return currencyValue;
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        // Convert currency value to string if T is string type
                        return (T)(object)documentField.Value.AsCurrency().Amount.ToString();
                    }
                    else
                    {
                        return default;
                    }
                default:
                    return default;
            }
        }
    }
}
