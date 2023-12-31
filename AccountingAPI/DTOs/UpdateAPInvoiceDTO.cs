﻿
namespace AccountingAPI.DTOs
{
    public class UpdateAPInvoiceDTO
    {
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public List<UpdateAPInvoiceLineDTO> InvoiceLines { get; set; }

        public UpdateAPInvoiceDTO()
        {
            InvoiceLines = new List<UpdateAPInvoiceLineDTO>();
            RefNumber = string.Empty;
            Currency = string.Empty;
        }
    }
}
