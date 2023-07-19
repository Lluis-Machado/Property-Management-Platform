using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Utilities;

namespace DocumentAnalyzerAPI.Mappers
{
    public class APInvoiceLineDTOMapper : IAPInvoiceLineDTOMapper
    {

        public APInvoiceLineDTO MapToAPInvoiceLineDTO(IReadOnlyDictionary<string, DocumentField> documentFields)
        {
            decimal? unitPrice = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "UnitPrice");
            int? quantity = AzureFormRecgonizerUtilities.MapFieldValue<int?>(documentFields, "Quantity");
            if (unitPrice is null || quantity is null)
            {
                unitPrice = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "Amount");
                quantity = 1;
            }

            APInvoiceLineDTO aPInvoiceLineDTO = new()
            {
                UnitPrice = unitPrice,
                Quantity = quantity,
                Tax = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "Tax"),
                Description = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "Description"),
            };
            

            return aPInvoiceLineDTO;
        }
    }
}
