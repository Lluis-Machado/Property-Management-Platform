﻿namespace AccountingAPI.Models
{
    public class ExpenseCategory : BaseModel
    {
        public string Name { get; set; }
        public string ExpenseTypeCode { get; set; }
        public int DepreciationPercent { get; set; }

        public ExpenseCategory()
        {
            Name = string.Empty;
            ExpenseTypeCode = string.Empty;
        }
    }
}
