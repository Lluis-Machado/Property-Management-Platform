﻿using System.Text.Json.Serialization;

namespace AccountingAPI.Models
{
    public class APInvoice :Invoice
    {
        public Guid VendorId { get; set; }

        [JsonConstructor]
        public APInvoice()
        {
        }
    }
}
