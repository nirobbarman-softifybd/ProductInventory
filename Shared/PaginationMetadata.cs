namespace ProductInventory.Shared
{
    public class PaginationMetadata
    {
        public int TotalCount { get; set; }
        public int CurrentPageDataCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
