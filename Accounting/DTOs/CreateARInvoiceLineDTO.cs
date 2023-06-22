﻿
namespace AccountingAPI.DTOs
{
    public class CreateARInvoiceLineDTO
    {
        public string Description { get; set; }
        public double Tax { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public DateTime ServiceDateFrom { get; set; }
        public DateTime ServiceDateTo { get; set; }
    }
}