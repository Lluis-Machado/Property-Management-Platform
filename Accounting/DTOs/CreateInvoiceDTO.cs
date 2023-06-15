﻿using System.Text.Json.Serialization;

namespace AccountingAPI.DTOs
{
    public class CreateInvoiceDTO
    {
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }
        public List<CreateInvoiceLineDTO> InvoiceLines { get; set; }   

        [JsonConstructor]
        public CreateInvoiceDTO()
        {
            RefNumber = string.Empty;
            Currency = string.Empty;
            InvoiceLines = new List<CreateInvoiceLineDTO>();
        }
    }
}
