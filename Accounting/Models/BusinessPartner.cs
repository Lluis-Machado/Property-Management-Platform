﻿using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class BusinessPartner :IAuditable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string VATNumber { get; set; }
        public string AccountID { get; set; }
        public string Type { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }

        [JsonConstructor]
        public BusinessPartner()
        {
            Name = string.Empty;
            VATNumber = string.Empty;
            AccountID = string.Empty;
            Type = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
