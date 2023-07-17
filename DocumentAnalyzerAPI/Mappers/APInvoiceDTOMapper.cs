using AutoMapper;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Utilities;

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
                RefNumber = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields,"InvoiceId"),
                Date = AzureFormRecgonizerUtilities.MapFieldValue<DateTime?>(documentFields,"InvoiceDate"),
                Currency = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "Currency")
            };
            return aPInvoiceDTO;
        }

       
    }
}
