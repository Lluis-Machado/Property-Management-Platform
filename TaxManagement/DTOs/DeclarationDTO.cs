﻿using static TaxManagement.Models.Declaration;

namespace TaxManagementAPI.DTOs
{
    public class DeclarationDTO
    {
        public Guid Id { get; set; }
        public Guid DeclarantId { get; set; }
        public DeclarationStatus Status { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string CreatedByUser { get; set; } = string.Empty;
        public string LastUpdateByUser { get; set; } = string.Empty;

    }
}
