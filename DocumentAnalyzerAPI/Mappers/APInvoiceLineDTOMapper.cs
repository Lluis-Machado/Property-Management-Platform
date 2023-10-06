using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using AccountingAPI.DTOs;
using DocumentAnalyzerAPI.Utilities;
using System.Text.RegularExpressions;

namespace DocumentAnalyzerAPI.Mappers
{
    public class APInvoiceLineDTOMapper : IAPInvoiceLineDTOMapper
    {

        public APInvoiceLineDTO MapToAPInvoiceLineDTO(IReadOnlyDictionary<string, DocumentField> documentFields, decimal mainTax)
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
            if (!String.IsNullOrEmpty(taxString))
            {
                Regex.Replace(taxString, @"[^\d]", "");
                Decimal.TryParse(taxStringClean, out tax);
            }
            else
            {
                tax = mainTax;
            }
            var totalPrice = unitPrice * (decimal)quantity;
            // TODO: Change quantity to decimal type
            APInvoiceLineDTO aPInvoiceLineDTO = new()
            {
                UnitPrice = unitPrice ?? 0,
                Quantity = (int)quantity,
                TotalPrice = totalPrice ?? 0,
                Tax = tax,
                Description = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "Description"),
            };


            return aPInvoiceLineDTO;
        }
    }
}
