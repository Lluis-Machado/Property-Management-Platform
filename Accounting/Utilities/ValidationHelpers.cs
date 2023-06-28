namespace AccountingAPI.Utilities
{
    public static class ValidationHelpers
    {
        public static bool IsAValidYear(int year)
        {
            return year >= 1900 && year <= DateTime.Now.Year; // Modify the range if needed
        }

        public static bool IsAValidMonth(int month)
        {
            return month >= 1 && month <= 12;
        }
    }
}
