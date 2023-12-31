﻿using static AccountingAPI.Utilities.PeriodStatusCodes;

namespace AccountingAPI.DTOs
{
    public class PeriodDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public PeriodStatus Status { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModificationBy { get; set; }

        public PeriodDTO()
        {
            CreatedBy = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
