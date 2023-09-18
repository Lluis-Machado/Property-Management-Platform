using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Utilities;
using System.Text.RegularExpressions;

namespace DocumentAnalyzerAPI.Mappers
{
    public class APInvoiceLineDTOMapper : IAPInvoiceLineDTOMapper
    {

        public APInvoiceLineDTO MapToAPInvoiceLineDTO(IReadOnlyDictionary<string, DocumentField> documentFields)
        {
            decimal? unitPrice = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "UnitPrice");
            double? quantity = AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "Quantity");

            if (unitPrice is null || quantity is null)
            {
                unitPrice = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "Amount");
                quantity = 1;
            }
            decimal tax;
            string taxString = (string?)AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "TaxRate");
            string taxStringClean = "";
            if(!String.IsNullOrEmpty(taxString))
            {
                Regex.Replace(taxString, @"[^\d]", "");

            }
            Decimal.TryParse(taxStringClean, out tax);
            var totalPrice = unitPrice * (decimal)quantity;
            APInvoiceLineDTO aPInvoiceLineDTO = new()
            {
                UnitPrice = unitPrice,
                Quantity = quantity,
                TotalPrice = totalPrice,
                Tax = tax,
                Description = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "Description"),
            };
            

            return aPInvoiceLineDTO;
        }
    }
}
