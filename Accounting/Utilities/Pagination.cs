namespace AccountingAPI.Utilities
{
    public static class Pagination
    {
        public static void Paginate<T>(ref IEnumerable<T> collection, int? page, int? pageSize)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                collection = collection.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
        }
    }
}
