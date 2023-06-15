﻿using System.Text.Json.Serialization;

namespace AccountingAPI.DTOs
{
    public class CreateExpenseCategoryDTO
    {
        public string? Name { get; set; }
        public string? ExpenseTypeCode { get; set; }
        public int DepreciationPercent { get; set; }

        [JsonConstructor]
        public CreateExpenseCategoryDTO()
        {
        }
    }
}
