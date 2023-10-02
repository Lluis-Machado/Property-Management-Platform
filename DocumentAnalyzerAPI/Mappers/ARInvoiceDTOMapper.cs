using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Utilities;
using AccountingAPI.DTOs;


namespace DocumentAnalyzerAPI.Mappers
{
    public class ARInvoiceDTOMapper : IARInvoiceDTOMapper
    {
        private readonly IARInvoiceLineDTOMapper _aRInvoiceLineDTOMapper;

        public ARInvoiceDTOMapper(IARInvoiceLineDTOMapper aRInvoiceLineDTOMapper)
        {
            _aRInvoiceLineDTOMapper = aRInvoiceLineDTOMapper;
        }

        public ARInvoiceDTO MapToARInvoiceAndLinesDTO(IReadOnlyDictionary<string, DocumentField> documentFields)
        {
            BusinessPartnerDTO businessPartnerDTO = new()
            {
                VATNumber = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "CustomerTaxId"),
                Name = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "CustomerName")
            };

            ARInvoiceDTO aPInvoiceDTO = new()
            {
                BusinessPartner = businessPartnerDTO,
                RefNumber = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "InvoiceId"),
                Date = AzureFormRecgonizerUtilities.MapFieldValue<DateTimeOffset?>(documentFields, "InvoiceDate")?.DateTime,
                Currency = AzureFormRecgonizerUtilities.MapFieldValue<string?>(documentFields, "InvoiceTotal"),
                TotalAmount = (decimal?)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "InvoiceTotal")
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
                        ARInvoiceLineDTO aRInvoiceLineDTO = _aRInvoiceLineDTOMapper.MapToARInvoiceLineDTO(itemFields);
                        aRInvoiceLineDTO.ServiceDateFrom = serviceDateFrom;
                        aRInvoiceLineDTO.ServiceDateTo = serviceDateTo;
                        aPInvoiceDTO.InvoiceLines.Add(aRInvoiceLineDTO);
                    }
                }

            }

            // if no invoice lines detected, create one with total values
            if (!aPInvoiceDTO.InvoiceLines.Any())
            {
                ARInvoiceLineDTO aRInvoiceLineDTO = new()
                {
                    Tax = (decimal)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "TotalTax"),
                    Quantity = 1,
                    UnitPrice = (decimal)AzureFormRecgonizerUtilities.MapFieldValue<double?>(documentFields, "InvoiceTotal")
                };
                aPInvoiceDTO.InvoiceLines.Add(aRInvoiceLineDTO);
            };

            return aPInvoiceDTO;
        }


    }
}
