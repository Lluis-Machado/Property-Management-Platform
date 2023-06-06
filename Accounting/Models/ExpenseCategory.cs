﻿namespace Accounting.Models
{
    public class ExpenseCategory :IAuditable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ExpenseTypeCode { get; set; }
        public int DepreciationPercent { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }

        public ExpenseCategory()
        {
            Name = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
