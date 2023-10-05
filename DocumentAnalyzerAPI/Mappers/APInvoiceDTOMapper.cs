using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using AccountingAPI.DTOs;
using DocumentAnalyzerAPI.Utilities;

namespace DocumentAnalyzerAPI.Mappers
{
    public class APInvoiceDTOMapper : IAPInvoiceDTOMapper
    {
        private readonly IAPInvoiceLineDTOMapper _aPInvoiceLineDTOMapper;

        public APInvoiceDTOMapper(IAPInvoiceLineDTOMapper aPInvoiceLineDTOMapper)
        {
            _aPInvoiceLineDTOMapper = aPInvoiceLineDTOMapper;
        }

        public APInvoiceDTO MapToAPInvoiceAndLinesDTO(IReadOnlyDictionary<string, DocumentField> documentFields)
        {
            BusinessPartnerDTO businessPartnerDTO = new()
            {
                VATNumber = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "VendorTaxId"),
                Name = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "VendorName")
            };

            var totalAmount = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "InvoiceTotal");
            var totalTax = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "TotalTax");
            var totalBaseAmount = totalAmount - totalTax;
            var totalTaxPercentage = (totalAmount *100)/totalBaseAmount -100;
            if (totalTaxPercentage != null) 
            totalTaxPercentage = Math.Round((decimal)totalTaxPercentage, 2);
            APInvoiceDTO aPInvoiceDTO = new()
            {
                BusinessPartner = businessPartnerDTO,
                RefNumber = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "InvoiceId"),
                Date = AzureFormRecgonizerUtilities.MapFieldValue<DateTimeOffset?>(documentFields, "InvoiceDate")?.DateTime,
                Currency = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "InvoiceTotal"),
                TotalAmount = totalAmount,
                TotalTax = totalTax,
                TotalBaseAmount = totalBaseAmount,
                TotalTaxPercentage = totalTaxPercentage
            };

            DateTime? serviceDateFrom = AzureFormRecgonizerUtilities.MapFieldValue<DateTimeOffset?>(documentFields, "ServiceStartDate")?.DateTime;
            DateTime? serviceDateTo = AzureFormRecgonizerUtilities.MapFieldValue<DateTimeOffset?>(documentFields, "ServiceEndDate")?.DateTime;

            if (documentFields.TryGetValue("Items", out var itemsField))
            {
                if (itemsField.FieldType == DocumentFieldType.List)
                {
                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {
                        IReadOnlyDictionary<string, DocumentField>? itemFields = itemField.Value.AsDictionary();
                        APInvoiceLineDTO aPInvoiceLineDTO = _aPInvoiceLineDTOMapper.MapToAPInvoiceLineDTO(itemFields);
                        aPInvoiceLineDTO.ServiceDateFrom = serviceDateFrom;
                        aPInvoiceLineDTO.ServiceDateTo = serviceDateTo;
                        aPInvoiceDTO.InvoiceLines.Add(aPInvoiceLineDTO);
                    }
                }
            }

            // if no invoice lines detected, create one with total values
            if (!aPInvoiceDTO.InvoiceLines.Any())
            {
                APInvoiceLineDTO aPInvoiceLineDTO = new()
                {
                    Tax = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "TotalTax"),
                    Quantity = 1,
                    UnitPrice = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "InvoiceTotal")
                };
                aPInvoiceDTO.InvoiceLines.Add(aPInvoiceLineDTO);
            };

            return aPInvoiceDTO;
        }


    }
}
